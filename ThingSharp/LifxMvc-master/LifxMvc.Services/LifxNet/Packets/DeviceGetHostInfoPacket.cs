using LifxMvc.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxNet
{
	public class DeviceGetHostInfoPacket : LifxPacketBase<DeviceStateHostInfoResponse>
	{
		override public PacketType MessageType { get { return PacketType.DeviceGetHostInfo; } }
		public DeviceGetHostInfoPacket(IBulb bulb) : base(bulb)
		{
			this.Header.ResponseRequired = true;
		}

		public DeviceGetHostInfoPacket(FrameHeader header)
			: base(header)
		{ }

	}
}
