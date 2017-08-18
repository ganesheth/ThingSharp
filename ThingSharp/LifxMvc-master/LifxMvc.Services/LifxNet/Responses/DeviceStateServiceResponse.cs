using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxNet
{
	/// <summary>
	/// Response to GetService message.
	/// Provides the device Service and port.
	/// If the Service is temporarily unavailable, then the port value will be 0.
	/// </summary>
	public class DeviceStateServiceResponse : LifxResponseBase
	{
		public DeviceStateServiceResponse(FrameHeader header, byte[] payload) : base()
		{
			this.Header = header;
			Service = payload[0];
			Port = BitConverter.ToUInt32(payload, 1);
		}
		public Byte Service { get; set; }
		public UInt32 Port { get; private set; }

		public byte[] TargetMacAddress { get { return this.Header.TargetMacAddress; } }
	}
}
