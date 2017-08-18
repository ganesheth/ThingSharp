using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxNet
{
	public class FrameHeader
	{
		static UInt32 _nextSource = 1;
		UInt32 _source = ++_nextSource;

		static byte _nextSequence = 1;
		byte _sequence = ++_nextSequence;

		public UInt32 Source;
		public bool Tagged;
		public byte Sequence;
		public bool AcknowledgeRequired;
		public bool ResponseRequired;
		public byte[] TargetMacAddress;
		public DateTime AtTime;
		public FrameHeader(bool acknowledgeRequired = false)
		{
			if (0 == _source)
			{
				++_source;
				++_nextSource; 
			}
			if (0 == _sequence)
			{
				++_sequence;
				++_nextSequence;
			}
			Source = _source;
			Sequence = _sequence;
			AcknowledgeRequired = acknowledgeRequired;
			ResponseRequired = false;
			TargetMacAddress = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
			AtTime = DateTime.MinValue;
			this.Tagged = false;

		}

		public void WriteToStream(BinaryWriter writer, byte[] payload)
		{
			#region Frame
			//size uint16
			writer.Write((UInt16)((payload != null ? payload.Length : 0) + 36)); //length

#if true
			UInt16 protocol = 0x1400;
			const UInt16 tagged = 1 << 13; //Account for littleEndian
			if (this.Tagged)
				protocol |= tagged;
			writer.Write((UInt16)protocol); //protocol

#else
				// origin (2 bits, must be 0), 
				// tagged (1 bit), Determines usage of the Frame Address target field.
				//The tagged field is a boolean flag that indicates whether the Frame Address target field is being used to address an individual device or all devices. For discovery using Device::GetService the tagged field should be set to one (1) and the target should be all zeroes. In all other messages the tagged field should be set to zero (0) and the target field should contain the device MAC address. The device will then respond with a Device::StateService message, which will include its own MAC address in the target field. In all subsequent messages that the client sends to the device, the target field should be set to the device MAC address, and the tagged field should be set to zero (0).

				// addressable (1 bit, must be 1), 
				//protocol 12 bits must be 0x400) = 0x1400
				writer.Write((UInt16)0x3400); //protocol
				//writer.Write((UInt16)0x1400); //protocol

#endif


			writer.Write((UInt32)this.Source); //source identifier - unique value set by the client, used by responses. If 0, responses are broadcasted instead
			Debug.Assert(0 != this.Source);

			#endregion Frame

			#region Frame address

			//The target device address is 8 bytes long, when using the 6 byte MAC address then left - 
			//justify the value and zero-fill the last two bytes. A target device address of all zeroes effectively addresses all devices on the local network
			writer.Write(this.TargetMacAddress); // target mac address - 0 means all devices


			//Let's play by the rules.
			writer.Write(new byte[] { 0, 0, 0, 0, 0, 0 }); //reserved 1


			//The client can use acknowledgements to determine that the LIFX device has received a message. 
			//However, when using acknowledgements to ensure reliability in an over-burdened lossy network ... 
			//causing additional network packets may make the problem worse. 
			//Client that don't need to track the updated state of a LIFX device can choose not to request a 
			//response, which will reduce the network burden and may provide some performance advantage. In
			//some cases, a device may choose to send a state update response independent of whether res_required is set.
			if (this.AcknowledgeRequired && this.ResponseRequired)
				writer.Write((byte)0x03);
			else if (this.AcknowledgeRequired)
				writer.Write((byte)0x02);
			else if (this.ResponseRequired)
				writer.Write((byte)0x01);
			else
				writer.Write((byte)0x00);
			//The sequence number allows the client to provide a unique value, which will be included by the LIFX 
			//device in any message that is sent in response to a message sent by the client. This allows the client
			//to distinguish between different messages sent with the same source identifier in the Frame. See
			//ack_required and res_required fields in the Frame Address.
			writer.Write(this.Sequence);
			
			#endregion Frame address

			#region Protocol Header
			//The at_time value should be zero for Set and Get messages sent by a client.
			//For State messages sent by a device, the at_time will either be the device
			//current time when the message was received or zero. StateColor is an example
			//of a message that will return a non-zero at_time value
			if (this.AtTime > DateTime.MinValue)
			{
				var time = this.AtTime.ToUniversalTime();
				writer.Write((UInt64)(time - new DateTime(1970, 01, 01)).TotalMilliseconds * 10); //timestamp
			}
			else
			{
				writer.Write((UInt64)0);
			}

			#endregion Protocol Header


		}

		public byte[] GetBytes(UInt16 payloadLength, PacketType messageType)
		{
			var result = this.GetBytes(payloadLength, (UInt16)messageType);
			return result;
		}

		public byte[] GetBytes(UInt16 payloadLength, ResponseType messageType)
		{
			var result = this.GetBytes(payloadLength, (UInt16)messageType);
			return result;
		}

		public const int FRAME_HEADER_LENGTH = 36;
		public UInt16 PacketLength { get; private set; }

		public byte[] GetBytes(UInt16 payloadLength, UInt16 messageType)
		{
			List<byte> bytes = new List<byte>();

			#region Frame, 64 bits/ 8 bytes

			
			#region Packet size (2 bytes/ 16 bits)

			const UInt16 FRAME_HEADER_LENGTH = 36;
			payloadLength += FRAME_HEADER_LENGTH;
			bytes.AddRange(BitConverter.GetBytes(payloadLength)); //length

			#endregion Packet size (2 bytes/ 16 bits)

			#region Origin/Tagged/Addressable/Protocol (2 bytes/ 2, 1, 1, 12 bits)

			const UInt16 ORIGIN = 0x00;
			const UInt16 PROTOCOL = 0x400;
			const UInt16 TAGGED_BIT = 1 << 13;
			const UInt16 ADDRESSABLE_BIT = 0x1000;

			UInt16 otap = ORIGIN;
			otap |= PROTOCOL;
			otap |= ADDRESSABLE_BIT;

			if (this.Tagged)
				otap |= TAGGED_BIT;
			bytes.AddRange(BitConverter.GetBytes(otap)); //protocol

			#endregion Origin/Tagged/Addressable/Protocol (2 bytes/ 2, 1, 1, 12 bits)

			#region Source identifier (4 bytes/ 32 bits)

			//Source identifier - unique value set by the client, 
			//used by responses. If 0, responses are broadcast instead of unicast.
			bytes.AddRange(BitConverter.GetBytes(this.Source));
			Debug.Assert(0 != this.Source); //DON'T BROADCAST.

			#endregion Source identifier (4 bytes/ 32 bits)

			
			#endregion Frame


			#region Frame address, 16 bytes/ 128 bits

			#region Target MAC address (8 bytes)

			//The target device address is 8 bytes long, when using the 6 byte MAC address then left - 
			//justify the value and zero-fill the last two bytes. A target device address of all zeroes effectively addresses all devices on the local network
			bytes.AddRange(this.TargetMacAddress); // target mac address - 0 means all devices

			#endregion Target MAC address (8 bytes)


			#region Reserved (6 bytes/ 48 bits)

			bytes.AddRange(new byte[] { 0, 0, 0, 0, 0, 0 }); //reserved 1

			#endregion Reserved (6 bytes/ 48 bits)


			#region Reserved, ResponseRequired, AckRequired (1 byte/ 6, 1, 1 bits)

			//The client can use acknowledgements to determine that the LIFX device has received a message. 
			//However, when using acknowledgements to ensure reliability in an over-burdened lossy network ... 
			//causing additional network packets may make the problem worse. 
			//Client that don't need to track the updated state of a LIFX device can choose not to request a 
			//response, which will reduce the network burden and may provide some performance advantage. In
			//some cases, a device may choose to send a state update response independent of whether res_required is set.
			const int RESPONSE_REQUIRED_BIT = 1;
			const int ACK_REQUIRED_BIT = 1 << 1;

			byte reserved = 0;

			if (this.ResponseRequired)
				reserved |= RESPONSE_REQUIRED_BIT;
			if (this.AcknowledgeRequired)
				reserved |= ACK_REQUIRED_BIT;

			bytes.Add(reserved);

			#endregion Reserved, ResponseRequired, AckRequired (1 byte/ 6, 1, 1 bits)


			#region Sequence (1 byte/ 8 bits)

			//The sequence number allows the client to provide a unique value, which will be included by the LIFX 
			//device in any message that is sent in response to a message sent by the client. This allows the client
			//to distinguish between different messages sent with the same source identifier in the Frame. See
			//ack_required and res_required fields in the Frame Address.
			bytes.Add(this.Sequence);

			#endregion Sequence (1 byte/ 8 bits)

			#endregion Frame address

			
			#region Protocol Header, 12 bytes/ 96 bits


			#region Reserved, sort of. (8 bytes/ 64 bits)
			//The at_time value should be zero for Set and Get messages sent by a client.
			//For State messages sent by a device, the at_time will either be the device
			//current time when the message was received or zero. StateColor is an example
			//of a message that will return a non-zero at_time value
			if (this.AtTime > DateTime.MinValue)
			{
				var time = this.AtTime.ToUniversalTime();
				bytes.AddRange(BitConverter.GetBytes((UInt64)(time - new DateTime(1970, 01, 01)).TotalMilliseconds * 10)); //timestamp
			}
			else
			{
				bytes.AddRange(BitConverter.GetBytes((UInt64)0));
			}

			#endregion Reserved, sort of. (8 bytes/ 64 bits)

			#region Message type (2 bytes/ 16 bits)

			bytes.AddRange(BitConverter.GetBytes(messageType));

			#endregion Message type (2 bytes/ 16 bits)

			#region Reserved (2 bytes/ 16 bits)

			bytes.AddRange(BitConverter.GetBytes((UInt16)0));

			#endregion Reserved (2 bytes/ 16 bits)


			#endregion Protocol Header


			var result = bytes.ToArray();
			return result;
		}


		public static FrameHeader Parse(byte[] packet)
		{
			var header = new FrameHeader();
			using (MemoryStream ms = new MemoryStream(packet))
			{
				BinaryReader br = new BinaryReader(ms);
				//frame
				header.PacketLength = br.ReadUInt16();
				if (packet.Length != header.PacketLength || header.PacketLength < FRAME_HEADER_LENGTH)
					throw new Exception("Invalid packet");
				var a = br.ReadUInt16(); //origin:2, reserved:1, addressable:1, protocol:12
				var source = br.ReadUInt32();
				header.Source = source;
				//frame address
				byte[] target = br.ReadBytes(8);
				header.TargetMacAddress = target;
				ms.Seek(6, SeekOrigin.Current); //skip reserved
				var b = br.ReadByte(); //reserved:6, ack_required:1, res_required:1, 
				header.Sequence = br.ReadByte();

				//protocol header
				var nanoseconds = br.ReadUInt64();
				header.AtTime = Constants.Epoch.AddMilliseconds(nanoseconds * 0.000001);
				header.MessageType = br.ReadUInt16();
				ms.Seek(2, SeekOrigin.Current); //skip reserved
			}

			return header;
		}

		public UInt16 MessageType { get; private set; }


	}//class
}//ns
