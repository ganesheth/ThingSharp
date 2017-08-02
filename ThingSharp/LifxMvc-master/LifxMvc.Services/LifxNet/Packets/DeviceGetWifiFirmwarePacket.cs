using LifxMvc.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxNet
{
	public class DeviceGetWifiFirmwarePacket : LifxPacketBase<DeviceStateWifiFirmwareResponse>
	{
		override public PacketType MessageType { get { return PacketType.DeviceGetWifiFirmware; } }
		public DeviceGetWifiFirmwarePacket(IBulb bulb) : base(bulb)
		{
			this.Header.ResponseRequired = true;
		}

		public DeviceGetWifiFirmwarePacket(FrameHeader header)
			: base(header)
		{ }
	}
}
