using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVCI_Extension
{
	class Core
	{
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
							result = "exit";
							break;
						case "get": // Empties the queue and sends instructions to Arma
							result = "get";
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

		#region Public Properties

		public static VoiceRecognizer Recognizer
		{ get; private set; }

		#endregion

		#region Core

		private static void Initialize()
		{
			Recognizer = new VoiceRecognizer();
		}

		#endregion
	}
}
