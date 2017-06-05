using LifxMvc.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxNet
{
	public class DeviceSetPowerPacket : LifxPacketBase<DeviceAcknowledgementResponse>
	{
		override public PacketType MessageType { get { return PacketType.DeviceSetPower; } }
		public bool IsOn { get; private set; }
		public DeviceSetPowerPacket(IBulb bulb, bool isOn)
			: base(bulb)

		{

			this.IsOn = isOn;
			this.Header.AcknowledgeRequired = true;
		}

		public DeviceSetPowerPacket(FrameHeader header, byte[] payload)
			: base(header)
		{
			IsOn = BitConverter.ToUInt16(payload, 0) > 0;
		}

		override protected object[] GetPayloadParams()
		{
			UInt16 level = this.IsOn ? UInt16.MaxValue : (UInt16)0U;
			return new object[] { level };
		}
	}
}
