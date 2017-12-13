using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VACI_Plugin
{
	class Logging
	{
		private static string LogDir = "./Apps/A3_VACI/";

		public static void Message(string source, string message)
		{
#if DEBUG
			{
				try
				{
					File.AppendAllText(LogDir + "Log_Messages.log", string.Concat(new string[]
					{
						DateTime.Now.ToString(),
						":  ",
						source,
						"	-- ",
						message,
						Environment.NewLine
					}));
				}
				catch (Exception)
				{
					Logging.Error("Logging", "Message log cannot be written to");
				}
			}
#endif
		}

		public static void Error(string source, string error)
		{
			File.AppendAllText(LogDir + "Log_Errors.log", string.Concat(new string[]
				{
					DateTime.Now.ToString(),
					":  ",
					source,
					"	-- ",
					error,
					Environment.NewLine
				}));
		}
	}
}
