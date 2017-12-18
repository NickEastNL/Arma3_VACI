using System;
using System.Collections.Concurrent;

namespace AVCI_Extension
{
	class Core
	{
		public static ConcurrentQueue<string> outQueue = new ConcurrentQueue<string>();

		#region Handling

		public static string HandleCall(string function)
		{
			string result = string.Empty;

			try
			{
				if (!string.IsNullOrEmpty(function))
				{
					switch (function)
					{
						case "init": // Initializes Extension
							Initialize();
							result = "init";
							break;
						case "exit": // Stops everything
							Shutdown();
							result = "exit";
							break;
						case "get": // Empties the queue and sends instructions to Arma
							result = GetInstruction();
							break;
						case "start-listen": // Start listening
							result = "start-listen";
							break;
						case "stop-listen": // Stop listening
							result = "stop-listen";
							break;
						default:
							break;
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Error("HandleCall", "Error: " + ex.Message);
			}

			return result;
		}

		#endregion

		private static string GetInstruction()
		{
			string result = string.Empty;

			try
			{
				string item;

				bool success = outQueue.TryDequeue(out item);

				if (success)
				{
					result = item;
					Logging.Message("GetInstruction", item);
				}
			}
			catch (Exception ex)
			{
				Logging.Error("GetInstruction", "Failed with error: " + ex.Message);
			}

			return result;
		}

		#region Public Properties

		public static VoiceRecognizer Recognizer
		{ get; private set; }

		#endregion

		#region Core

		private static void Initialize()
		{
			if (Recognizer == null)
			{
				Recognizer = new VoiceRecognizer();
			}
			else
			{
				Logging.Message("Init", "Voice Recognizer already running");
			}
		}

		private static void Shutdown()
		{
			if (Recognizer != null)
			{
				Recognizer.Dispose();
				Recognizer.Dispose();
			}
		}

		#endregion
	}
}
