using LifxMvc.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxNet
{
	public class DeviceGetServicePacket : LifxPacketBase<DeviceStateServiceResponse>
	{
		override public PacketType MessageType { get { return PacketType.DeviceGetService; } }

		public DeviceGetServicePacket()
			: base((Bulb)null)
		{
			this.Header.AcknowledgeRequired = false;
			this.Header.Tagged = true;
			this.Header.Source = UInt32.MaxValue;
		}

		public DeviceGetServicePacket(FrameHeader header)
			: base(header)
		{
		}
	}
}
