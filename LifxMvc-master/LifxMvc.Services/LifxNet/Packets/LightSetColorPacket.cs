using LifxMvc.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxNet
{
	public class LightSetColorPacket : LifxPacketBase<LightStateResponse>
	{
		override public PacketType MessageType { get { return PacketType.LightSetColor; } }

		public byte Reserved { get; private set; }
		public UInt16 Hue { get; private set; }
		public UInt16 Saturation { get; private set; }
		public UInt16 Brightness { get; private set; }
		public UInt16 Kelvin { get; private set; }

		public UInt32 Duration { get; set; }

		public LightSetColorPacket(IBulb bulb, IHSBK hsbk)
			: base(bulb)
		{
			if (null == hsbk)
				throw new ArgumentNullException();

			this.Hue = hsbk.Hue;
			this.Saturation = hsbk.Saturation;
			this.Brightness = hsbk.Brightness;
			this.Kelvin = hsbk.Kelvin;

			this.Header.ResponseRequired = true;
		}
		public LightSetColorPacket(FrameHeader header, byte[] payload)
			: base(header)
		{
			Reserved = payload[0];
			Hue = BitConverter.ToUInt16(payload, 1);
			Saturation = BitConverter.ToUInt16(payload, 3);
			Brightness = BitConverter.ToUInt16(payload, 5);
			Kelvin = BitConverter.ToUInt16(payload, 7);
			Duration = BitConverter.ToUInt32(payload, 9);
		}


		override protected object[] GetPayloadParams()
		{
			return new object[] { this.Reserved, this.Hue, this.Saturation, this.Brightness, this.Kelvin, this.Duration };
		}
	}
}
