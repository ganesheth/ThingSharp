using LifxNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LifxNet
{
	static public class Extensions
	{
		[Conditional("TRACE_PACKETS")]
		static public void TraceSent(this LifxPacketBase packet, EndPoint localEndPoint)
		{
			var msg = string.Format("{0} -> {1}: {2}",
				localEndPoint.ToString().PadRight(18),
				packet.IPEndPoint.ToString().PadRight(18), 
				packet
				);
			Debug.WriteLine(msg);
		}

		[Conditional("TRACE_PACKETS")]
		static public void TraceReceived(this LifxResponseBase response, EndPoint localEndPoint, bool unexpectedResponse = false)
		{
			if (response is LightStateResponse)
				new object();

			var mac = string.Format(" : MacAddress={0}:{1}:{2}:{3}:{4}:{5}",
				response.Header.TargetMacAddress[0].ToString("X2"),
				response.Header.TargetMacAddress[1].ToString("X2"),
				response.Header.TargetMacAddress[2].ToString("X2"),
				response.Header.TargetMacAddress[3].ToString("X2"),
				response.Header.TargetMacAddress[4].ToString("X2"),
				response.Header.TargetMacAddress[5].ToString("X2"));

			var msg = string.Format("{0} <- {1}: {2}{3}{4}{5}",
				localEndPoint.ToString().PadRight(18),
				response.IPEndPoint.ToString().PadRight(18),
				unexpectedResponse ? "***" : string.Empty,
				response,
				unexpectedResponse ? "***" : string.Empty,
				unexpectedResponse ? mac : string.Empty
				);

			Debug.WriteLine(msg);
		}
	}
}
