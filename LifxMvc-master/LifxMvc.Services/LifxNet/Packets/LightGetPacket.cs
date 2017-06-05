using LifxMvc.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxNet
{
	public class LightGetPacket : LifxPacketBase<LightStateResponse>
	{
		override public PacketType MessageType { get { return PacketType.LightGet; } }
		public LightGetPacket(IBulb bulb)
			: base(bulb)
		{
			
		}

		public LightGetPacket(FrameHeader header)
			: base(header)
		{ }
	}
}
