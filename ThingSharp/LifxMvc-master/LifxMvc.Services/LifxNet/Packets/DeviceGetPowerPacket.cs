using LifxMvc.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxNet
{
	public class DeviceGetPowerPacket : LifxPacketBase<DeviceStatePowerResponse>
	{
		override public PacketType MessageType { get { return PacketType.DeviceGetPower; } }
		public DeviceGetPowerPacket(IBulb bulb) : base(bulb)
		{
			this.Header.ResponseRequired = true;
		}

		public DeviceGetPowerPacket(FrameHeader header)
			: base(header)
		{ }
	}
}
