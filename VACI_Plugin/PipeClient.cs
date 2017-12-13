using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VACI_Plugin
{
	class PipeClient
	{
		static string pipeName = "VACI";

		private static Thread pipeSenderThread;
		private static ConcurrentQueue<string> outQueue = new ConcurrentQueue<string>();

		public static void StartThread()
		{
			if (pipeSenderThread == null)
			{
				pipeSenderThread = new Thread(new ThreadStart(RunPipeClient));
				pipeSenderThread.IsBackground = true;
				pipeSenderThread.Start();
				Logging.Message("Client", "Pipe sender thread started");
			}
		}

		public static string Transmit(string message)
		{
			Logging.Message("Transmit", "Preparing to transmit message");
			Task.Run(delegate
			{
				try
				{
					Logging.Message("Transmit", "Async transmit attempting to start");
					if (string.IsNullOrEmpty(message))
					{
						Logging.Message("Transmit", "Message is empty");
					}
					else
					{
						outQueue.Enqueue(message);
						Logging.Message("Transmit", "Message added to queue and ready for transmition");
					}
				}
				catch (Exception ex)
				{
					Logging.Error("Transmit", ex.Message);
				}
			}).ConfigureAwait(false);

			return message;
		}

		private static void RunPipeClient()
		{
			try
			{
				Logging.Message("Client", "Pipe client starting");
				while (true)
				{
					using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut))
					{
						Logging.Message("Client", "Connecting to pipe server");
						try
						{
							pipeClient.Connect(1000);
						}
						catch (Exception ex)
						{
							Logging.Message("Client", "Connection failed: " + ex.Message);
						}

						if (pipeClient.IsConnected)
						{
							pipeClient.ReadMode = PipeTransmissionMode.Message;
							Logging.Message("Client", "Connected");
							while (pipeClient.IsConnected)
							{
								try
								{
									string queueItem = string.Empty;
									bool isDequeued = outQueue.TryDequeue(out queueItem);
									if (!isDequeued)
									{
										continue;
									}
									if (queueItem == null)
									{
										continue;
									}
									
									bool isSent = SendPipeMessage(pipeClient, queueItem);
									if (isSent)
									{
										Logging.Message("Client", "Message sent");
										continue;
									}
								}
								catch (Exception ex)
								{
									Logging.Error("Client", ex.Message);
								}
								break;
							}
						}
					}
					Logging.Message("Client", "Connection dropped or failed");
				}
			}
			catch (Exception ex2)
			{
				Logging.Message("Client", "Thread failed with error: " + ex2.Message);
			}
		}

		private static bool SendPipeMessage(NamedPipeClientStream pipe, string message)
		{
			bool result = false;

			try
			{
				Logging.Message("Message", "Sending message: " + message);
				byte[] bytes = Encoding.UTF8.GetBytes(message);
				pipe.Write(bytes, 0, bytes.Length);
				result = true;
			}
			catch (Exception ex)
			{
				Logging.Error("Message", ex.Message);
				result = false;
			}

			return result;
		}
	}
}
