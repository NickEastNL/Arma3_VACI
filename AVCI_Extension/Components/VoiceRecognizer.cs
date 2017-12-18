using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;

namespace AVCI_Extension
{
	class VoiceRecognizer
	{
		#region Private Members

		private SpeechRecognitionEngine Engine;

		#endregion

		#region Constructor

		public VoiceRecognizer()
		{
			if (SpeechRecognitionEngine.InstalledRecognizers().Count == 0)
			{
				Logging.Error("Recognizer", "It appears you have no Speech Recognizer pack installed.");
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

				Engine = new SpeechRecognitionEngine(speechRecognizer);

				try
				{
					Engine.SetInputToDefaultAudioDevice();
					Logging.Message("Recognizer", "Using default input device.");
				}
				catch (InvalidOperationException ex)
				{
					Logging.Error("Recognizer", "Check input device. Error: " + ex.Message);
					return;
				}

				// TODO: Load the grammar

				// TODO: Register event handlers
			}
			catch (Exception ex)
			{
				Logging.Error("Recognizer", String.Format("{0}\nCurrent Culture: {1}\nAvailable Recognizers: {2}\nStack Trace:\n{3}", ex.Message, CultureInfo.InstalledUICulture.Name, SpeechRecognitionEngine.InstalledRecognizers().Select(x => x.Culture.Name).Aggregate((x, y) => x + ", " + y), ex.StackTrace));
			}

			Logging.Message("Recognizer", "Speech Recognizer set up correctly");
		}

		#endregion
	}
}
