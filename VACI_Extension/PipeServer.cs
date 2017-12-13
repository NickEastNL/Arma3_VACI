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
		static ConcurrentQueue<string> inQueue = new ConcurrentQueue<string>();

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
							Logging.Message("Call", "Return value: " + text);
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

							inQueue.Enqueue(pipeMessage);
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

		private static string Get()
		{
			string result;

			string text = string.Empty;

			while (!inQueue.IsEmpty)
			{
				string queueItem = string.Empty;
				bool deQueue = inQueue.TryDequeue(out queueItem);
				if (deQueue)
				{
					text = queueItem;
				}
			}

			result = text;
			if (!string.IsNullOrEmpty(result))
			{
				Logging.Message("Get", "Message returns: " + result);
			}
			return result;
		}
	}
}
