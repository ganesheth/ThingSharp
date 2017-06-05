using LifxNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxMvc
{
	public class UnknownPacket : LifxPacketBase
	{
		public override PacketType MessageType
		{
			get
			{
				return PacketType.Unknown;
			}
		}

		public UnknownPacket(FrameHeader header, byte[] payload)
			: base(header)
		{
			var msg = string.Format("Unknown packet type; {0}", this.Header.MessageType);
			throw new ArgumentOutOfRangeException(msg);
		}


	}
}
