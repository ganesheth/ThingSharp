using LifxMvc.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxNet
{
	public class DeviceGetLocationPacket : LifxPacketBase<DeviceStateLocationResponse>
	{
		override public PacketType MessageType { get { return PacketType.DeviceGetLocation; } }
		public DeviceGetLocationPacket(IBulb bulb) : base(bulb)
		{
			this.Header.ResponseRequired = true;
		}

		public DeviceGetLocationPacket(FrameHeader header)
			: base(header)
		{ }
	}
}
