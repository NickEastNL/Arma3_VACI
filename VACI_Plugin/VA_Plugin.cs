using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VACI_Plugin
{
    public class VA_Plugin
    {
        public static string VA_DisplayName()
        {
            return "VoiceAttack Command Interface for Arma 3";
        }

        public static Guid VA_Id()
        {
            return new Guid("{8c220fcb-e41e-4b3d-87b3-34ea46321b48}");
        }

        public static string VA_DisplayInfo()
        {
            return "VoiceAttack Command Interface (VACI) for Arma 3\r\n\r\nA plugin intended to be used in combination with the Arma 3 VACI mod (found on Steam Workshop). This plugin provides functionality to send instructions to the mod to provide integrated voice commands and remove the need for using keybindings.";
        }

        public static void VA_Init1(dynamic vaProxy)
        {
			PipeClient.StartThread();
		}

        public static void VA_Exit1(dynamic vaProxy)
        {
            //Shut down the plugin (e.g. close the Pipe Server and threads to clean up)
        }

        public static void VA_Invoke1(dynamic vaProxy)
        {
            string message = PipeClient.Transmit(vaProxy.Context);
#if DEBUG
			vaProxy.WriteToLog(message, "purple");
#endif
		}

		public static void VA_StopCommand()
		{

		}
		
    }
}
