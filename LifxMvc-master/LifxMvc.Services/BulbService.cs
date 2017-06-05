using LifxMvc.Domain;
using LifxMvc.Services.UdpHelper;
using LifxNet;
using LifxNet.Domain;
using System;
using System.Drawing;

namespace LifxMvc.Services
{
	public class BulbService : IBulbService
	{
		R Send<R>(IBulb bulb, LifxPacketBase<R> packet) where R : LifxResponseBase
		{
			var udp = UdpHelperManager.Instance[packet.IPEndPoint];
			var response = udp.Send(packet);
			if (null != response)
				BulbExtensions.Set(bulb, (dynamic)response);
			return response;
		}

		void SendAsync(IBulb bulb, LifxPacketBase packet) 
		{
			var udp = UdpHelperManager.Instance[packet.IPEndPoint];
			udp.SendAsync(packet);
		}


		public void Initialize(IBulb bulb)
		{
			this.DeviceGetVersion(bulb);
			this.LightGet(bulb);
			this.DeviceGetGroup(bulb);
			this.DeviceGetLocation(bulb);
		}

		public void LightGet(IBulb bulb)
		{
			var packet = new LightGetPacket(bulb);
			this.Send(bulb, packet);
		}

		public void DeviceGetGroup(IBulb bulb)
		{
			var packet = new DeviceGetGroupPacket(bulb);
			this.Send(bulb, packet);
		}
		public void DeviceGetLocation(IBulb bulb)
		{
			var packet = new DeviceGetLocationPacket(bulb);
			this.Send(bulb, packet);
		}



		public bool DeviceGetPower(IBulb bulb)
		{
			var packet = new DeviceGetPowerPacket(bulb);
			var response = this.Send(bulb, packet);

			var result = response.IsOn;
			return result;
		}

		public void DeviceSetPower(IBulb bulb, bool isOn)
		{
			var packet = new DeviceSetPowerPacket(bulb, isOn);
			this.Send(bulb, packet);

			bulb.IsOn = isOn;
		}

		public void DeviceGetVersion(IBulb bulb)
		{
			var packet = new DeviceGetVersionPacket(bulb);
			this.Send(bulb, packet);
		}

		
		public void GetHostInfo(IBulb bulb)
		{
			var packet = new DeviceGetHostInfoPacket(bulb);
			this.Send(bulb, packet);
		}
		public void GetHostFirmware(IBulb bulb)
		{
			var packet = new DeviceGetHostFirmwarePacket(bulb);
			this.Send(bulb, packet);
		}
		public void GetWifiInfo(IBulb bulb)
		{
			var packet = new DeviceGetWifiInfoPacket(bulb);
			this.Send(bulb, packet);
		}
		public void GetWifiFirmware(IBulb bulb)
		{
			var packet = new DeviceGetWifiFirmwarePacket(bulb);
			this.Send(bulb, packet);
		}
		public void GetLabel(IBulb bulb)
		{
			var packet = new DeviceGetLabelPacket(bulb);
			this.Send(bulb, packet);
		}
		public void SetLabel(IBulb bulb, string label)
		{
			var packet = new DeviceSetLabelPacket(bulb, label);
			this.Send(bulb, packet);
		}

		public void GetInfo(IBulb bulb)
		{
			var packet = new DeviceGetInfoPacket(bulb);
			this.Send(bulb, packet);
		}
		public void EchoRequest(IBulb bulb)
		{
			var packet = new DeviceEchoRequestPacket(bulb);
			this.Send(bulb, packet);
		}
		public bool LightGetPower(IBulb bulb)
		{
			var packet = new LightGetPowerPacket(bulb);
			this.Send(bulb, packet);
			return bulb.IsOn;
		}
		public void LightSetPower(IBulb bulb, bool power)
		{
			var packet = new LightSetPowerPacket(bulb, power);
			this.Send(bulb, packet);
			bulb.IsOn = power;
		}

		public void LightSetWaveform(IBulb bulb, LightSetWaveformCreationContext ctx)
		{
			var packet = new LightSetWaveformPacket(bulb, ctx);
			this.SendAsync(bulb, packet);
		}

		void LightSetColor(IBulb bulb, IHSBK hsbk)
		{
			var packet = new LightSetColorPacket(bulb, hsbk);
			packet.Duration = 100;
			this.SendAsync(bulb, packet);

			bulb.SetColor(hsbk);
		}

		public void LightSetColor(IBulb bulb, Color color)
		{
			var hsbk = color.ToHSBK(bulb.IsKelvin);
			LightSetColor(bulb, hsbk);
		}




	}//class

	public static class BulbExtensions
	{
		public static void SetColor(this IBulb bulb, IHSBK hsbk)
		{
			bulb.Color = hsbk.ToColor();

			bulb.Hue = hsbk.Hue;
			bulb.Saturation = hsbk.Saturation;
			bulb.Brightness = hsbk.Brightness;
			bulb.Kelvin = hsbk.Kelvin;
		}

		public static void Set(this IBulb bulb, DeviceAcknowledgementResponse r)
		{
		}

		public static void Set(this IBulb bulb, DeviceEchoResponse r)
		{
		}

		public static void Set(this IBulb bulb, DeviceStateGroupResponse r)
		{
			bulb.Group = r.Label;
		}

		public static void Set(this IBulb bulb, DeviceStateHostFirmwareResponse r)
		{
			bulb.HostFirmwareBuild = r.Build;
			bulb.HostFirmwareVersion = r.Version;
		}

		public static void Set(this IBulb bulb, DeviceStateHostInfoResponse r)
		{
			bulb.Signal = r.Signal;
			bulb.TxCount = r.TxCount;
			bulb.RxCount = r.RxCount;
		}

		public static void Set(this IBulb bulb, DeviceStateInfoResponse r)
		{
			bulb.Time = r.Time;
			bulb.Uptime = r.Uptime;
			bulb.Downtime = r.Downtime;
		}

		public static void Set(this IBulb bulb, DeviceStateLabelResponse r)
		{
			bulb.Label = r.Label;
		}
		
		public static void Set(this IBulb bulb, DeviceStateLocationResponse r)
		{
			bulb.Location = r.Label;
		}

		public static void Set(this IBulb bulb, DeviceStatePowerResponse r)
		{
			bulb.IsOn = r.IsOn;
		}

		
		public static void Set(this IBulb bulb, DeviceStateServiceResponse r)
		{
			bulb.Service = r.Service;
			bulb.Port = r.Port;
		}
		
		public static void Set(this IBulb bulb, DeviceStateVersionResponse r)
		{
			bulb.Vendor = r.Vendor;
			bulb.Product = (LifxProductEnum)r.Product;
			bulb.Version = r.Version;
		}

		public static void Set(this IBulb bulb, DeviceStateWifiFirmwareResponse r)
		{
			bulb.WifiFirmwareBuild = r.Build;
			bulb.WifiFirmwareVersion = r.Version;
		}

		public static void Set(this IBulb bulb, DeviceStateWifiInfoResponse r)
		{
			bulb.WifiInfoSignal = r.Signal;
			bulb.WifiInfoTxCount = r.TxCount;
			bulb.WifiInfoRxCount = r.RxCount;
		}

		
		public static void Set(this IBulb bulb, LightStatePowerResponse r)
		{
			bulb.IsOn = r.IsOn;
		}

		public static void Set(this IBulb bulb, LightStateResponse r)
		{
			bulb.IsOn = r.IsOn;
			bulb.Label = r.Label;

			bulb.Hue = r.Hue;
			bulb.Saturation = r.Saturation;
			bulb.Brightness = r.Brightness;
			bulb.Kelvin = r.Kelvin;

			IHSBK hsbk = null;
			if (bulb.Product.IsColor())
				hsbk = new HSBK(r.Hue, r.Saturation, r.Brightness);
			else
				hsbk = new HSBK(r.Kelvin, r.Brightness);

			bulb.SetHSBK(hsbk);
		}

		public static void Set(this IBulb bulb, UnknownResponse r)
		{
			throw new ArgumentOutOfRangeException();
		}
		



	}//class

}//ns
