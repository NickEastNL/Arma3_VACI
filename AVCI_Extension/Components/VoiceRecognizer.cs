using System;
using System.Globalization;
using System.Linq;
using System.Speech.Recognition;
using System.Threading;

namespace AVCI_Extension
{
	public class CommandDetectedEventArgs : EventArgs
	{
		public CommandDetectedEventArgs(string phrase, float confidence) : base()
		{
			Phrase = phrase;
			Confidence = confidence;
		}

		public string Phrase { get; private set; }

		public float Confidence { get; private set; }
	}

	class VoiceRecognizer : IDisposable
	{
		#region Private Members

		private SpeechRecognitionEngine Engine;

		private Object ConfidenceLock;

		private AutoResetEvent EngineShuttingDown;

		#endregion

		#region Public Members

		public enum VoiceRecognizerState
		{
			Error,
			Listening,
			LinsteningOnce,
			Paused,
			Pausing,
		}

		public VoiceRecognizerState State { get; private set; }

		public int ConfidenceMargin
		{
			get { return Engine != null ? (int)Engine.QueryRecognizerSetting("CFGConfidenceRejectionThreshold") : 90; }
			set
			{
				if (Engine != null)
				{
					ChangeConfidence(value);
				}
			}
		}

		public int EndSilenceTimeout
		{
			get
			{
				return Engine != null ? (int)Engine.EndSilenceTimeout.TotalMilliseconds : 0;
			}
			set
			{
				if (Engine == null) return;

				Engine.EndSilenceTimeout = TimeSpan.FromMilliseconds(value);
			}
		}

		#endregion

		#region Events

		public event EventHandler<CommandDetectedEventArgs> CommandAccepted;

		public event EventHandler<CommandDetectedEventArgs> CommandRejected;

		public event EventHandler StartedListening;

		public event EventHandler StoppedListening;

		#endregion

		#region Constructor

		public VoiceRecognizer()
		{
			if (SpeechRecognitionEngine.InstalledRecognizers().Count == 0)
			{
				Logging.Error("Recognizer", "It appears you have no Speech Recognizer pack installed.");
				State = VoiceRecognizerState.Error;
				return;
			}

			try
			{
				var installedRecognizers = SpeechRecognitionEngine.InstalledRecognizers();
				RecognizerInfo speechRecognizer = null;

				switch (CultureInfo.InstalledUICulture.Name)
				{
					case "en-GB":
						speechRecognizer = (installedRecognizers.FirstOrDefault(x => x.Culture.Name == "en-GB") ?? installedRecognizers.FirstOrDefault(x => x.Culture.TwoLetterISOLanguageName == "en"));
						break;
					case "en-US":
						speechRecognizer = installedRecognizers.FirstOrDefault(x => x.Culture.TwoLetterISOLanguageName == "en");
						break;
					default:
						speechRecognizer = installedRecognizers.FirstOrDefault(x => x.Culture.TwoLetterISOLanguageName == "en");
						break;
				}

				if (speechRecognizer == null)
				{
					Logging.Error("Recognizer", String.Format("You don't appear to have the {0} Speech Recognizer installed. Articulate requires this recognizer to be present in order to function correctly.", "English"));
					return;
				}

				ConfidenceLock = new Object();
				EngineShuttingDown = new AutoResetEvent(false);
				State = VoiceRecognizerState.Paused;

				Engine = new SpeechRecognitionEngine(speechRecognizer);

				try
				{
					Engine.SetInputToDefaultAudioDevice();
					Logging.Message("Recognizer", "Using default input device.");
				}
				catch (InvalidOperationException ex)
				{
					Logging.Error("Recognizer", "Check input device. Error: " + ex.Message);
					State = VoiceRecognizerState.Error;
					return;
				}

				ConfidenceMargin = 90;
				
				Grammar g = new Grammar(CommandList.BuildSrgsGrammar(speechRecognizer.Culture));
				Engine.LoadGrammar(g);
				
				Engine.SpeechRecognized += SpeechRecognized;
				Engine.SpeechRecognitionRejected += SpeechRecognitionRejected;
				Engine.RecognizeCompleted += RecognizeCompleted;

				StartListening();
			}
			catch (Exception ex)
			{
				Logging.Error("Recognizer", String.Format("{0}\nCurrent Culture: {1}\nAvailable Recognizers: {2}\nStack Trace:\n{3}", ex.Message, CultureInfo.InstalledUICulture.Name, SpeechRecognitionEngine.InstalledRecognizers().Select(x => x.Culture.Name).Aggregate((x, y) => x + ", " + y), ex.StackTrace));
				State = VoiceRecognizerState.Error;
				return;
			}

			Logging.Message("Recognizer", "Speech Recognizer set up correctly");
		}

		#endregion

		#region Private Methods
		private void ChangeConfidence(int value)
		{
			lock (ConfidenceLock)
			{
				Engine.UpdateRecognizerSetting("CFGConfidenceRejectionThreshold", value);
			}
		}

		private void TriggerCommandAccepted(string phrase, float confidence)
		{
			if (CommandAccepted != null)
			{
				CommandAccepted(this, new CommandDetectedEventArgs(phrase, confidence));
			}
		}

		private void TriggerCommandRejected(string phrase, float confidence)
		{
			if (CommandRejected != null)
			{
				CommandRejected(this, new CommandDetectedEventArgs(phrase, confidence));
			}
		}

		private void TriggerStartedListening()
		{
			if (StartedListening != null)
			{
				StartedListening(this, EventArgs.Empty);
			}
		}

		private void TriggerStoppedListening()
		{
			if (StoppedListening != null)
			{
				StoppedListening(this, EventArgs.Empty);
			}
		}
		#endregion

		#region Public Methods
		public void StartListening()
		{
			if (State == VoiceRecognizerState.Pausing)
			{
				EngineShuttingDown.WaitOne();
			}

			if (Engine != null && State == VoiceRecognizerState.Paused)
			{
				Engine.RecognizeAsync(RecognizeMode.Multiple);
				State = VoiceRecognizerState.Listening;
				TriggerStartedListening();
			}
		}

		public void StopListening()
		{
			if (Engine != null && (State == VoiceRecognizerState.Listening || State == VoiceRecognizerState.LinsteningOnce))
			{
				Engine.RecognizeAsyncStop();
				State = VoiceRecognizerState.Pausing;
				TriggerStoppedListening();
			}
		}

		public void CancelListening()
		{
			if (Engine != null && (State == VoiceRecognizerState.Listening || State == VoiceRecognizerState.LinsteningOnce))
			{
				Engine.RecognizeAsyncCancel();
				State = VoiceRecognizerState.Pausing;
				TriggerStoppedListening();
			}
		}

		public void ListenOnce()
		{
			if (State == VoiceRecognizerState.Pausing)
			{
				EngineShuttingDown.WaitOne();
			}

			if (Engine != null && State == VoiceRecognizerState.Paused)
			{
				Engine.RecognizeAsync(RecognizeMode.Single);
				State = VoiceRecognizerState.LinsteningOnce;
				TriggerStartedListening();
			}
		}
		#endregion

		#region SpeechRecognitionEngine Events
		private void SpeechRecognized(object sender, SpeechRecognizedEventArgs recognizedPhrase)
		{
			TriggerCommandAccepted(recognizedPhrase.Result.Words.Aggregate("", (phraseSoFar, word) => phraseSoFar + word.Text + " "), recognizedPhrase.Result.Confidence);

			CommandList.Execute(recognizedPhrase.Result.Semantics);
		}

		private void SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs recognizedPhrase)
		{
			TriggerCommandRejected(recognizedPhrase.Result.Words.Aggregate("", (phraseSoFar, word) => phraseSoFar + word.Text + " "), recognizedPhrase.Result.Confidence);
		}

		private void RecognizeCompleted(object sender, RecognizeCompletedEventArgs args)
		{
			State = VoiceRecognizerState.Paused;

			EngineShuttingDown.Set();
		}
		#endregion

		public void Dispose()
		{
			Engine.RecognizeAsyncCancel();

			Engine.Dispose();
			Engine = null;
		}
	}
}
