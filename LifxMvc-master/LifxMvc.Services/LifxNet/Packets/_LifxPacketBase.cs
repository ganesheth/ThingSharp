using LifxMvc;
using LifxMvc.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LifxNet
{
	public abstract class LifxPacketBase<R> : LifxPacketBase where R : LifxResponseBase
	{
		public Type ExpectedResponseType { get { return typeof(R); } }
		public LifxPacketBase(IBulb bulb)
			: base(bulb)
		{ }

		public LifxPacketBase(FrameHeader header)
			: base(header)
		{ }

	}

	public abstract class LifxPacketBase
	{
		#region Fields

		private byte[] _payload;
		private ushort _type;

		#endregion

		#region Properties

		public FrameHeader Header { get; protected set; }
		public IPEndPoint IPEndPoint { get; set; }
		public abstract PacketType MessageType { get; }

		/// <summary>
		/// ///////////////////////////////////////////////////////////
		/// </summary>
		public byte[] Payload { get { return _payload; } }
		public ushort Type { get { return (ushort)this.MessageType; } }


		#endregion        
		
		#region Construction

		protected LifxPacketBase(IBulb bulb)
		{
			this.Header = new FrameHeader();
			if (null != bulb)
			{
				this.IPEndPoint = bulb.IPEndPoint;
				this.Header.TargetMacAddress = bulb.TargetMacAddress;
			}
		}

		protected LifxPacketBase(FrameHeader header)
		{
			this.Header = header;
		}

		#endregion

		public byte[] Serialize()
		{
			List<byte> bytes = new List<byte>();

			var payload = this.GetPayloadBytes();
			UInt16 payloadLength = (UInt16)payload.Length;
			var header = this.Header.GetBytes(payloadLength, this.MessageType);

			using (var ms = new MemoryStream())
			{
				using (BinaryWriter writer = new BinaryWriter(ms))
				{
					writer.Write(header);
					if (payload != null)
					{
						writer.Write(payload);
					}
					bytes.AddRange(ms.ToArray());
				}
			}

			var result = bytes.ToArray();
			return result;
		}

		public byte[] GetPayloadBytes()
		{
			object[] args = this.GetPayloadParams();
			List<byte> bytes = new List<byte>();

			if (args != null)
			{
				foreach (var arg in args)
				{
					if (arg is byte)
						bytes.Add((byte)arg);
					else if (arg is byte[])
						bytes.AddRange((byte[])arg);
					else if (arg is string)
					{
						var chars = ((string)arg).PadRight(32, (char)0).Take(32).ToArray();
						var charBytes = Encoding.UTF8.GetBytes(chars);
						bytes.AddRange(charBytes); //All strings are 32 bytes
					}
					else
					{
						bytes.AddRange(BitConverter.GetBytes((dynamic)arg));
					}
				}
			}

			byte[] result = bytes.ToArray();
			return result;
		}

		virtual protected object[] GetPayloadParams()
		{
			return null;
		}

		public override string ToString()
		{
			var result = string.Format("Source={1}, Sequence={2} : {0}",
					base.ToString(),
					this.Header.Source.ToString("X8"),
					this.Header.Sequence.ToString("X2"));
			return result;
		}

	}//class


	static public class PacketFactory
	{
		public static LifxPacketBase Parse(byte[] packet, IPEndPoint sender)
		{
			var header = FrameHeader.Parse(packet);
			using (MemoryStream ms = new MemoryStream(packet))
			{
				BinaryReader br = new BinaryReader(ms);
				br.ReadBytes(FrameHeader.FRAME_HEADER_LENGTH);// Fast forward past the FrameHeader.

				byte[] payload = null;
				if (header.PacketLength > FrameHeader.FRAME_HEADER_LENGTH)
					payload = br.ReadBytes(header.PacketLength - FrameHeader.FRAME_HEADER_LENGTH);
				return PacketFactory.Create(header, payload, sender);
			}
		}

		public static LifxPacketBase Create(FrameHeader header, byte[] payload, IPEndPoint sender)
		{
			LifxPacketBase packet = null;
			PacketType type = (PacketType)header.MessageType;
			switch (type)
			{

				case PacketType.DeviceGetService:
					packet = new DeviceGetServicePacket(header);
					break;
				case PacketType.DeviceGetHostInfo:
					packet = new DeviceGetHostInfoPacket(header);
					break;
				case PacketType.DeviceGetHostFirmware:
					packet = new DeviceGetHostFirmwarePacket(header);
					break;
				case PacketType.DeviceGetWifiInfo:
					packet = new DeviceGetWifiInfoPacket(header);
					break;
				case PacketType.DeviceGetWifiFirmware:
					packet = new DeviceGetWifiFirmwarePacket(header);
					break;
				case PacketType.DeviceGetPower:
					packet = new DeviceGetPowerPacket(header);
					break;
				case PacketType.DeviceSetPower:
					packet = new DeviceSetPowerPacket(header, payload);
					break;
				case PacketType.DeviceGetLabel:
					packet = new DeviceGetLabelPacket(header);
					break;
				case PacketType.DeviceSetLabel:
					packet = new DeviceSetLabelPacket(header, payload);
					break;
				case PacketType.DeviceGetVersion:
					packet = new DeviceGetVersionPacket(header);
					break;
				case PacketType.DeviceGetInfo:
					packet = new DeviceGetInfoPacket(header);
					break;
				case PacketType.DeviceGetLocation:
					packet = new DeviceGetLocationPacket(header);
					break;
				case PacketType.DeviceGetGroup:
					packet = new DeviceGetGroupPacket(header);
					break;
				case PacketType.DeviceEchoRequest:
					packet = new DeviceEchoRequestPacket(header);
					break;
				case PacketType.LightGet:
					packet = new LightGetPacket(header);
					break;
				case PacketType.LightSetColor:
					packet = new LightSetColorPacket(header, payload);
					break;
				case PacketType.LightSetWaveform:
					packet = new LightSetWaveformPacket(header, payload);
					break;
				case PacketType.LightGetPower:
					packet = new LightGetPowerPacket(header);
					break;
				case PacketType.LightSetPower:
					packet = new LightSetPowerPacket(header, payload);
					break;

				default:
					packet = new UnknownPacket(header, payload);
					break;
			}
			packet.IPEndPoint = sender;
			return packet;
		}

	}



















}//ns