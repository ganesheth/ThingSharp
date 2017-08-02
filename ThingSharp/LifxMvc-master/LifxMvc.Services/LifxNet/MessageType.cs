using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxNet
{
	public enum PacketType : ushort
	{
		//Device requests
		DeviceGetService	= 0x02,
		DeviceGetHostInfo	= 12,  //0x0c
		DeviceGetHostFirmware = 14,//0x0e
		DeviceGetWifiInfo	= 16,  //0x10
		DeviceGetWifiFirmware = 18,//0x12
		DeviceGetPower		= 20,  //0x14
		DeviceSetPower		= 21,  //0x15
		DeviceGetLabel		= 23,  //0x17
		DeviceSetLabel		= 24,  //0x18
		DeviceGetVersion	= 32,  //0x20
		DeviceGetInfo		= 34,  //0x22
		DeviceGetLocation	= 48,  //0x30
		DeviceGetGroup		= 51,  //0x33
		DeviceEchoRequest	= 58,  //0x3a

		//Light messages
		LightGet		= 101,	   //0x65
		LightSetColor	= 102,	   //0x66
		LightSetWaveform= 103,	   //0x67
		LightGetPower	= 116,	   //0x74
		LightSetPower	= 117,	   //0x75


		//Unofficial
		LightGetTemperature = 0x6E,
		//LightStateTemperature = 0x6f,
		SetLightBrightness = 0x68,

		Unknown = ushort.MaxValue
	}

	public enum ResponseType : ushort
	{
		//Device responses
		DeviceStateService		= 0x03,
		DeviceStateTime			= 0x06,
		DeviceStateHostInfo		= 13, //0x0d
		DeviceStateHostFirmware = 15, //0x0f
		DeviceStateWifiInfo		= 17, //0x11
		DeviceStateWifiFirmware = 19, //0x13
		DeviceStatePower		= 22, //0xi6
		DeviceStateLabel		= 25, //0x19
		DeviceStateVersion		= 33, //0x21
		DeviceStateInfo			= 35, //0x23
		DeviceAcknowledgement	= 45, //0x2d
		DeviceStateLocation		= 50, //0x32
		DeviceStateGroup		= 53, //0x35
		DeviceEcho				= 59, //0x38
		LightState				= 107,//0x6b
		LightStatePower			= 118,//0x76


	}




}//ns
