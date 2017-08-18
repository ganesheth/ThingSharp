using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LifxNet
{
	/// <summary>
	/// Base class for LIFX response types
	/// </summary>
	public abstract class LifxResponseBase
	{
		public FrameHeader Header { get; set; }
		public byte[] Payload { get; protected set; }
		public ResponseType Type { get; set; }
		public UInt32 Source { get { return Header.Source; } }
		public byte Sequence { get { return Header.Sequence; } }
		public IPEndPoint IPEndPoint { get; set; }

		public LifxResponseBase() { }

		public override string ToString()
		{
			var result = string.Format("Source={1}, Sequence={2} : {0}",
					base.ToString(), 
					this.Header.Source.ToString("X8"), 
					this.Header.Sequence.ToString("X2"));
			return result;
		}
	}


	static public class ResponseFactory
	{
		public static LifxResponseBase Parse(byte[] packet, IPEndPoint sender)
		{
			var header = FrameHeader.Parse(packet);
			using (MemoryStream ms = new MemoryStream(packet))
			{
				BinaryReader br = new BinaryReader(ms);
				br.ReadBytes(FrameHeader.FRAME_HEADER_LENGTH);// Fast forward past the FrameHeader.

				byte[] payload = null;
				if (header.PacketLength > FrameHeader.FRAME_HEADER_LENGTH)
					payload = br.ReadBytes(header.PacketLength - FrameHeader.FRAME_HEADER_LENGTH);
				return ResponseFactory.Create(header, payload, sender);
			}
		}

		public static LifxResponseBase Create(FrameHeader header, byte[] payload, IPEndPoint sender)
		{
			LifxResponseBase response = null;
			ResponseType type = (ResponseType)header.MessageType;
			switch (type)
			{
				case ResponseType.DeviceAcknowledgement:
					response = new DeviceAcknowledgementResponse(payload);
					break;
				case ResponseType.DeviceStateLabel:
					response = new DeviceStateLabelResponse(payload);
					break;
				case ResponseType.LightState:
					response = new LightStateResponse(payload);
					break;
				case ResponseType.LightStatePower:
					response = new LightStatePowerResponse(payload);
					break;
				case ResponseType.DeviceStateVersion:
					response = new DeviceStateVersionResponse(payload);
					break;
				case ResponseType.DeviceStateHostFirmware:
					response = new DeviceStateHostFirmwareResponse(payload);
					break;
				case ResponseType.DeviceStateService:
					response = new DeviceStateServiceResponse(header, payload);
					break;

				case ResponseType.DeviceStatePower:
					response = new DeviceStatePowerResponse(payload);
					break;

				case ResponseType.DeviceEcho:
					response = new DeviceEchoResponse(payload);
					break;

				case ResponseType.DeviceStateHostInfo:
					response = new DeviceStateHostInfoResponse(payload);
					break;
				case ResponseType.DeviceStateWifiInfo:
					response = new DeviceStateWifiInfoResponse(payload);
					break;

				case ResponseType.DeviceStateTime:
					response = new DeviceStateTimeResponse(payload);
					break;

				case ResponseType.DeviceStateWifiFirmware:
					response = new DeviceStateWifiFirmwareResponse(payload);
					break;

				case ResponseType.DeviceStateInfo:
					response = new DeviceStateInfoResponse(payload);
					break;

				case ResponseType.DeviceStateLocation:
					response = new DeviceStateLocationResponse(payload);
					break;


				case ResponseType.DeviceStateGroup:
					response = new DeviceStateGroupResponse(payload);
					break;



				default:
					response = new UnknownResponse(payload);
					break;
			}
			response.Header = header;
			response.Type = type;
			response.IPEndPoint = sender;
			return response;
		}

	}


}//ns
