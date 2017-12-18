using RGiesecke.DllExport;
using System.Runtime.InteropServices;
using System.Text;

namespace AVCI_Extension
{
	public class DllEntry
	{
#if WIN64
		[DllExport("RVExtensionVersion", CallingConvention = CallingConvention.Winapi)]
#else
        [DllExport("_RVExtensionVersion@8", CallingConvention = CallingConvention.Winapi)]
#endif
		public static void RVExtensionVersion(StringBuilder output, int outputSize)
		{
			output.Append("1.0.0.0");
		}

#if WIN64
		[DllExport("RVExtension", CallingConvention = CallingConvention.Winapi)]
#else
        [DllExport("_RVExtension@12", CallingConvention = CallingConvention.Winapi)]
#endif
		public static void RVExtension(StringBuilder output, int outputSize, [MarshalAs(UnmanagedType.LPStr)] string function)
		{
			output.Append(Core.HandleCall(function));
		}
	}
}
