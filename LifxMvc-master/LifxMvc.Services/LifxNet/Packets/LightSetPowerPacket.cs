using LifxMvc.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxNet
{
	public class LightSetPowerPacket : LifxPacketBase<DeviceAcknowledgementResponse>
	{
		override public PacketType MessageType { get { return PacketType.LightSetPower; } }
		public bool IsOn { get; private set; }
		public UInt32 Duration { get; set; }

		public LightSetPowerPacket(IBulb bulb, bool isOn)
			: base(bulb)
		{
			this.IsOn = isOn;
			this.Header.AcknowledgeRequired = true;            
		}

		public LightSetPowerPacket(FrameHeader header, byte[] payload)
			: base(header)
		{
			IsOn = BitConverter.ToUInt16(payload, 0) > 0;
			Duration = BitConverter.ToUInt32(payload, 2);
		}

		override protected object[] GetPayloadParams()
		{
			UInt16 level = this.IsOn ? UInt16.MaxValue : (UInt16)0U;
			return new object[] { level, this.Duration };
		}
	}
}
