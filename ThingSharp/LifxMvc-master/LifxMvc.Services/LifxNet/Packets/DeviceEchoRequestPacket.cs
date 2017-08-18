using LifxMvc.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxNet
{
	public class DeviceEchoRequestPacket : LifxPacketBase<DeviceEchoResponse>
	{
		override public PacketType MessageType { get { return PacketType.DeviceEchoRequest; } }

		public DeviceEchoRequestPacket(IBulb bulb) : base(bulb)
		{
			this.Header.ResponseRequired = true;
		}

		public DeviceEchoRequestPacket(FrameHeader header)
			: base(header)
		{
		}

		override protected object[] GetPayloadParams()
		{
			var payload = new byte[64];
			return new object[] { payload };
		}

	}
}
