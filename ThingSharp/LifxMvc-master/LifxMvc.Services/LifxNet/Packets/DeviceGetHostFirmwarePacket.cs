using LifxMvc.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxNet
{
	public class DeviceGetHostFirmwarePacket : LifxPacketBase<DeviceStateHostFirmwareResponse>
	{
		override public PacketType MessageType { get { return PacketType.DeviceGetHostFirmware; } }
		public DeviceGetHostFirmwarePacket(IBulb bulb) : base(bulb)
		{
			this.Header.ResponseRequired = true;
		}

		public DeviceGetHostFirmwarePacket(FrameHeader header)
			: base(header)
		{ }
	}
}
