using LifxMvc.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxNet
{
	public class DeviceGetWifiInfoPacket : LifxPacketBase<DeviceStateWifiInfoResponse>
	{
		override public PacketType MessageType { get { return PacketType.DeviceGetWifiInfo; } }
		public DeviceGetWifiInfoPacket(IBulb bulb) : base(bulb)
		{
			this.Header.ResponseRequired = true;
		}

		public DeviceGetWifiInfoPacket(FrameHeader header)
			: base(header)
		{ }
	}
}
