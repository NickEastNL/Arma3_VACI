using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VACI_Extension
{
	class PipeServer
	{
		private class Instruction
		{
			public string Type = string.Empty;

			public string Parameter = string.Empty;

			public string Arguments = string.Empty;
		}

		static string pipeName = "VACI";
		static bool serverIsRunning = false;

		static Thread pipeServerThread = null;
		static ConcurrentQueue<Instruction> inQueue = new ConcurrentQueue<Instruction>();

		public static void StartThread()
		{
			if (pipeServerThread == null || !pipeServerThread.IsAlive)
			{
				Logging.Message("Server", "Starting server thread");
				pipeServerThread = new Thread(new ThreadStart(RunPipeServer));
				pipeServerThread.IsBackground = true;
				pipeServerThread.Start();
				serverIsRunning = true;
			}
		}

		public static void StopThread()
		{
			Logging.Message("Server", "Stopping server thread");
			pipeServerThread.Abort();
			serverIsRunning = false;
		}

		public static string HandleCall(string function)
		{
			//Logging.Message("Call", "Function: " + function);
			string text = string.Empty;
			
			try
			{
				//Logging.Message("Call", "Checking pipe server thread");
				StartThread();

				if (!string.IsNullOrEmpty(function))
				{

					if (serverIsRunning)
					{
						if (function == "get")
						{
							text = Get();

							if (!string.IsNullOrEmpty(text))
							{
								Logging.Message("Call", "Return value: " + text);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Error("Call", "Generic error: " + ex.Message);
			}

			return text;
		}

		private static void RunPipeServer()
		{
			while (true)
			{
				using (NamedPipeServerStream pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.InOut, -1, PipeTransmissionMode.Message))
				{
					Logging.Message("Server", "Pipe server listening");
					pipeServer.WaitForConnection();
					Logging.Message("Server", "Pipe server connected");

					while (true)
					{
						bool isConnected = pipeServer.IsConnected;
						if (!isConnected)
						{
							Logging.Message("Server", "No clients connected");
							break;
						}

						Logging.Message("Server", "Still connected. Ready to receive messages.");
						try
						{
							string pipeMessage = GetPipeMessage(pipeServer);
							if (string.IsNullOrEmpty(pipeMessage))
							{
								goto DISCONNECT;
							}

							bool isEvaluated = EvalPipeMessage(pipeMessage);

							if (!isEvaluated)
							{
								goto DISCONNECT;
							}
							Logging.Message("Server", "Message received and queued.");
						}
						catch (Exception ex)
						{
							Logging.Error("Server", ex.Message);
							goto DISCONNECT;
						}
					}
					Logging.Message("Server", "Not connected. Stopping attempt");
					DISCONNECT:
					Logging.Message("Server", "Send loop terminated. Disconnecting pipe");
					pipeServer.Disconnect();
				}
			}
		}
		
		private static string GetPipeMessage(NamedPipeServerStream pipe)
		{
			string text = string.Empty;

			try
			{
				StringBuilder stringBuilder = new StringBuilder();
				byte[] array = new byte[1024];

				do
				{
					int count = pipe.Read(array, 0, array.Length);
					stringBuilder.Append(Encoding.UTF8.GetString(array, 0, count));
					array = new byte[array.Length];
				} while (!pipe.IsMessageComplete);

				text = stringBuilder.ToString();
				Logging.Message("Receive", "Message reads: " + text);
			}
			catch (Exception)
			{
				text = "error reading message";
				Logging.Error("Receive", "Error: " + text);
			}

			return text.Trim();
		}

		private static bool EvalPipeMessage(string message)
		{
			bool result = false;

			string a = message.ToLower();

			try
			{
				if (!string.IsNullOrEmpty(a))
				{
					Logging.Message("Eval", "Message received and parsing: " + message);

					string[] array = message.Split(new string[] { ";" }, StringSplitOptions.None);
					Instruction instruction = new Instruction();

					string b = array[0].Trim();
					if (b == "code")
					{
						if (array.Length >= 2)
						{
							instruction.Type = array[0].Trim();
							instruction.Parameter = array[1].Trim();
						}
						else
						{
							Logging.Message("Eval", "Expecting a 'code' instruction with a single line of code.");
						}
					}
					else if (b == "script")
					{
						if (array.Length >= 2)
						{
							instruction.Type = array[0].Trim();
							instruction.Parameter = array[1].Trim() + ".sqf"; // Add proper folder path to the script files

							if (!string.IsNullOrEmpty(array[2]))
							{
								// Array string needs to be the following format to work "[""string"",0,true]" (no whitespace)
								// Preferably, the format which users have to enter in the "Context" field of VoiceAttack should be simpler.
								// e.g. ['string', 0, true], which should be stored as a string: "['string', 0, true]"
								string raw = array[2].Trim(); //The raw, untreated string

								if (raw[0] == '[' && raw[raw.Length - 1] == ']')
								{

								}
								else
								{
									Logging.Message("Eval", "Optional arguments not formatted properly: " + raw);
								}
							}
						}
						else
						{
							Logging.Message("Eval", "Expecting a 'script' instruction with filename and optionally an array of arguments");
						}
					}

					if (!string.IsNullOrEmpty(instruction.Type) && !string.IsNullOrEmpty(instruction.Parameter))
					{
						inQueue.Enqueue(instruction);
						Logging.Message("Eval", "Message added to queue");
					}

					result = true;
				}
				else
				{
					Logging.Message("Eval", "Message was empty");
				}
			}
			catch (Exception ex)
			{
				Logging.Error("Eval", "Error: " + ex.Message);
				result = false;
			}

			return result;
		}

		private static string Get()
		{
			string result = string.Empty;

			try
			{
				List<string> list = new List<string>();

				while (!inQueue.IsEmpty)
				{
					Instruction instruction = null;

					bool isDequeued = inQueue.TryDequeue(out instruction);
					if (isDequeued)
					{
						list.Add(string.Concat(new string[] {
							instruction.Type,
							"|",
							instruction.Parameter,
							string.IsNullOrEmpty(instruction.Arguments) ? "" : "|",
							instruction.Arguments
						}));
					}
					if (list.Count != 0)
					{
						result = string.Join(",", list.ToArray());
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Error("Get", "Error: " + ex.Message);
			}
			return result;
		}
	}
}
