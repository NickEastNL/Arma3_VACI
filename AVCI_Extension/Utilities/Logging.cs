using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVCI_Extension
{
	class Logging
	{
		public static void Message(string source, string message)
		{
#if DEBUG
			{
				try
				{
					File.AppendAllText("AVCILog_Messages.log", string.Concat(new string[]
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
					Error("Logging", "Message log cannot be written to");
				}
			}
#endif
		}

		public static void Error(string source, string error)
		{
			File.AppendAllText("AVCILog_Errors.log", string.Concat(new string[]
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
