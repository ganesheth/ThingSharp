using LifxMvc.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxNet
{
	public class DeviceGetInfoPacket : LifxPacketBase<DeviceStateInfoResponse>

	{
		override public PacketType MessageType { get { return PacketType.DeviceGetInfo; } }
		public DeviceGetInfoPacket(IBulb bulb) : base(bulb)
		{
			this.Header.ResponseRequired = true;
		}

		public DeviceGetInfoPacket(FrameHeader header)
			: base(header)
		{ }
	}
}
