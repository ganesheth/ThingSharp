using LifxMvc.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxNet
{
	public class DeviceSetLabelPacket : LifxPacketBase<DeviceAcknowledgementResponse>
	{
		override public PacketType MessageType { get { return PacketType.DeviceSetLabel; } }

		const int MAX_LENGTH = 32;
		string _label;
		public string Label
		{
			get { return _label ?? string.Empty; }
			set
			{
				if (null == value)
					value = string.Empty;
				if (MAX_LENGTH < value.Length)
					throw new ArgumentOutOfRangeException("Maximum Label length is 32.");
				_label = value;
			}
		}
		public DeviceSetLabelPacket(IBulb bulb, string label) : base(bulb)
		{
			this.Header.AcknowledgeRequired = true;
			this.Label = label;
		}

		public DeviceSetLabelPacket(FrameHeader header, byte[] payload)
			: base(header)
		{
			_label = Encoding.UTF8.GetString(payload, 0, 32);
		}


		override protected object[] GetPayloadParams()
		{
			//_label = _label.PadRight(MAX_LENGTH);
			return new object[] { _label };
		}
	}
}
