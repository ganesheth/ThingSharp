using LifxMvc.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxNet
{
	public class LightGetPowerPacket : LifxPacketBase<LightStatePowerResponse>
	{
		override public PacketType MessageType { get { return PacketType.LightGetPower; } }
		public LightGetPowerPacket(IBulb bulb) : base(bulb)
		{
			this.Header.ResponseRequired = true;
		}

		public LightGetPowerPacket(FrameHeader header)
			: base(header)
		{ }
	}
}
