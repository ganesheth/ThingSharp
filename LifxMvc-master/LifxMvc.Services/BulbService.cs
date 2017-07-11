using LifxMvc.Domain;
using LifxMvc.Services.UdpHelper;
using LifxNet;
using LifxNet.Domain;
using System;
using System.Drawing;
using System.Threading.Tasks;

namespace LifxMvc.Services
{
	public class BulbService : IBulbService
	{
        const int BULB_OFFLINE_RETRY_TIME = 10000;
        const int BULB_LAST_READ_RETRY_TIME = 10000;

		R Send<R>(IBulb bulb, LifxPacketBase<R> packet) where R : LifxResponseBase
		{
            var response = (R)null;            

            // If the bulb is Offline and we haven't waited 15 seconds yet, then skip any communication
            // with the bulb and send back a NULL response
            if (BulbOfflineCheck(bulb))
            {
                var udp = UdpHelperManager.Instance[packet.IPEndPoint];
                response = udp.Send(packet);

                if (null != response)
                {
                    bulb.isOffline = false;

                    // Don't update the bulb info if we sent a SET message because the response
                    // has the previous settings. For example, if we change brightness from 25%
                    // to 75%, the response will show brightness as 25%.
                    if (!IsSetPacket(packet.MessageType))
                        BulbExtensions.Set(bulb, (dynamic)response);
                }
                else
                {
                    //bulb.IsOn = false;
                    bulb.isOffline = true;
                    bulb.LastOfflineCheck = DateTime.UtcNow;
                }
            }

			return response;
		}
        //--------------------------------------------------------------------

        private bool IsSetPacket(PacketType pt)
        {
            bool isSetPacket = false;

            if(pt == PacketType.LightSetColor || pt == PacketType.LightSetPower || pt == PacketType.LightSetWaveform)
            {
                isSetPacket = true;
            }

            return isSetPacket;
        }
        //--------------------------------------------------------------------

        private bool BulbOfflineCheck(IBulb bulb)
        {
            bool okToSendRequestToBulb = true;

            // If the bulb is offline, then wait BULB_OFFLINE_RETRY_TIME seconds before checking if it's back online.
            if (bulb.isOffline)
            {
                double deltaTime = DateTime.UtcNow.Subtract(bulb.LastOfflineCheck).TotalMilliseconds;
                if (deltaTime > 0 && deltaTime < BULB_OFFLINE_RETRY_TIME)
                {
                    okToSendRequestToBulb = false;
                }
            }

            return okToSendRequestToBulb;
        }
        //--------------------------------------------------------------------

        private bool IsOkToReadFromBulb(DateTime lastReadTime)
        {
            bool okToRead = false;

            double deltaTime = DateTime.UtcNow.Subtract(lastReadTime).TotalMilliseconds;

            // If the time difference since the last time we read data from the light is negative, or
            // greater than BULB_LAST_READ_RETRY_TIME, then get the data from the light again.
            // Else, just use the old data
            if (deltaTime < 0 || deltaTime > BULB_LAST_READ_RETRY_TIME)
            {
                okToRead = true;
            }

            return okToRead;
        }
        //--------------------------------------------------------------------

		bool SendAsync(IBulb bulb, LifxPacketBase packet) 
		{
            bool success = false;

            //if (!bulb.isOffline)
            //{
            //    var udp = UdpHelperManager.Instance[packet.IPEndPoint];
            //    udp.SendAsync(packet);
            //    success = true;
            //}
            //else
            //{
            //    //return false;
            //}

            return success;
		}
        //--------------------------------------------------------------------

		public bool Initialize(IBulb bulb)
		{
			this.DeviceGetVersion(bulb);
			bool gotLightData = this.LightGet(bulb, true);
			//this.DeviceGetGroup(bulb);
			//this.DeviceGetLocation(bulb);

            return gotLightData;
		}
        //--------------------------------------------------------------------

		public bool LightGet(IBulb bulb, bool forceUpdate = false)
		{
            bool gotResponse = false;

            if (IsOkToReadFromBulb(bulb.LastStateRequest) || forceUpdate)
            {
                // update the Last Request time first so other threads that coming in 
                // immediatly don't try reading as well
                bulb.LastStateRequest = DateTime.UtcNow;

                var packet = new LightGetPacket(bulb);
                var response = this.Send(bulb, packet);

                

                if (response != null)
                {                    
                    gotResponse = true;
                }
                else
                {
                    // If there was an issue reading, then set date time Min Value so
                    // we try reading with the next request instead of waiting for the timeout.
                    bulb.LastStateRequest = DateTime.MinValue;
                }
            }

            return gotResponse;
		}
        //--------------------------------------------------------------------

		public void DeviceGetGroup(IBulb bulb)
		{            
			var packet = new DeviceGetGroupPacket(bulb);
			this.Send(bulb, packet);
		}
        //--------------------------------------------------------------------

		public void DeviceGetLocation(IBulb bulb)
		{
			var packet = new DeviceGetLocationPacket(bulb);
			this.Send(bulb, packet);
		}
        //--------------------------------------------------------------------

        public bool DeviceGetPower(IBulb bulb)
        {
            var packet = new DeviceGetPowerPacket(bulb);
            var response = this.Send(bulb, packet);

            var result = response.IsOn;
            return result;
        }
        //--------------------------------------------------------------------

		public void DeviceSetPower(IBulb bulb, bool isOn)
		{
			var packet = new DeviceSetPowerPacket(bulb, isOn);
			this.Send(bulb, packet);

			bulb.IsOn = isOn;
		}
        //--------------------------------------------------------------------

		public void DeviceGetVersion(IBulb bulb)
		{
			var packet = new DeviceGetVersionPacket(bulb);
			this.Send(bulb, packet);
		}
        //--------------------------------------------------------------------

		public void GetHostInfo(IBulb bulb)
		{
			var packet = new DeviceGetHostInfoPacket(bulb);
			this.Send(bulb, packet);
		}
        //--------------------------------------------------------------------

		public void GetHostFirmware(IBulb bulb)
		{
			var packet = new DeviceGetHostFirmwarePacket(bulb);
			this.Send(bulb, packet);
		}
        //--------------------------------------------------------------------

		public void GetWifiInfo(IBulb bulb)
		{
			var packet = new DeviceGetWifiInfoPacket(bulb);
			this.Send(bulb, packet);
		}
        //--------------------------------------------------------------------

		public void GetWifiFirmware(IBulb bulb)
		{
			var packet = new DeviceGetWifiFirmwarePacket(bulb);
			this.Send(bulb, packet);
		}
        //--------------------------------------------------------------------

		public void GetLabel(IBulb bulb)
		{
			var packet = new DeviceGetLabelPacket(bulb);
			this.Send(bulb, packet);
		}
        //--------------------------------------------------------------------

		public void SetLabel(IBulb bulb, string label)
		{
			var packet = new DeviceSetLabelPacket(bulb, label);
			this.Send(bulb, packet);
		}
        //--------------------------------------------------------------------

		public void GetInfo(IBulb bulb)
		{
			var packet = new DeviceGetInfoPacket(bulb);
			this.Send(bulb, packet);
		}
        //--------------------------------------------------------------------

		public void EchoRequest(IBulb bulb)
		{
			var packet = new DeviceEchoRequestPacket(bulb);
			this.Send(bulb, packet);
		}
        //--------------------------------------------------------------------

        public uint? LightGetPower(IBulb bulb)
        {
            // Only read from the bulb every 15 seconds. This helps eliminate
            // every property asking the bulb for the same info.
            if (IsOkToReadFromBulb(bulb.LastPowerRequest))
            {
                // update the Last Request time first so other threads that coming in 
                // immediatly don't try reading as well
                bulb.LastPowerRequest = DateTime.UtcNow;

                var packet = new LightGetPowerPacket(bulb);
                var response = this.Send(bulb, packet);
                if (response == null)
                {
                    // If there was an issue reading, then set date time Min Value so
                    // we try reading with the next request instead of waiting for the timeout.
                    bulb.LastPowerRequest = DateTime.MinValue;
                }
            }

            return bulb.isOffline ? null : (uint?)Convert.ToInt32(bulb.IsOn);
        }
        //--------------------------------------------------------------------

		public bool LightSetPower(IBulb bulb, bool power)
		{
			bool isSuccess = false;

            var packet = new LightSetPowerPacket(bulb, power); 
			var response = this.Send(bulb, packet);


            if (response != null)
            {
                isSuccess = true;
                bulb.IsOn = power;
            }

            return isSuccess;
		}
        //--------------------------------------------------------------------

		public void LightSetWaveform(IBulb bulb, LightSetWaveformCreationContext ctx)
		{
			var packet = new LightSetWaveformPacket(bulb, ctx);
			//this.SendAsync(bulb, packet);
		}
        //--------------------------------------------------------------------

        bool LightSetHSBK(IBulb bulb)
		{
            bool isSuccess = false;

            var packet = new LightSetHSBKPacket(bulb);
			packet.Duration = 100;

            var response = this.Send(bulb, packet);

            if (response != null)
            {
                isSuccess = true;
            }

            return isSuccess;
		}
        //--------------------------------------------------------------------

        public string LightGetColor(IBulb bulb)
        {
            // Get the latest Bulb HSBK settings 
            this.LightGet(bulb);

            string colorString = String.Format("{0:X2}{1:X2}{2:X2}{3:X2}", bulb.Color.A, bulb.Color.R, bulb.Color.G, bulb.Color.B);

            return bulb.isOffline ? null : colorString;
        }
        //--------------------------------------------------------------------

		public bool LightSetColor(IBulb bulb, Color color)
		{
            // Get the latest Bulb HSBK settings (could have change from another source)
            this.LightGet(bulb, true);

            // create new HSBK from color
			var hsbk = color.ToHSBK(bulb.IsKelvin);
            if (null == hsbk)
                throw new ArgumentNullException();

            // Update the Bulb's Hue            
            bulb.Hue = hsbk.Hue;
            bulb.Saturation = hsbk.Saturation;
            bulb.Brightness = hsbk.Brightness;

            // Create the Packet and send request
            return LightSetHSBK(bulb);
		}
        //--------------------------------------------------------------------

        public ushort? LightGetBrightness(IBulb bulb)
        {
            // Get the latest Bulb HSBK settings (could have change from another source)
            this.LightGet(bulb);
            return bulb.isOffline ? null : (ushort?)bulb.Brightness;
        }
        //--------------------------------------------------------------------

        public bool LightSetBrightness(IBulb bulb, ushort brightness)
        {
            // Get the latest Bulb color settings (could have change from another source)
            this.LightGet(bulb, true);

            // Update the brightness setting 
            bulb.Brightness = brightness;

            // Create the Packet and send request
            return LightSetHSBK(bulb);
        }
        //--------------------------------------------------------------------

        public ushort? LightGetSaturation(IBulb bulb)
        {
            // Get the latest Bulb HSBK settings (could have change from another source)
            this.LightGet(bulb);
            return bulb.isOffline ? null : (ushort?)bulb.Saturation;
        }
        //--------------------------------------------------------------------

        public bool LightSetSaturation(IBulb bulb, ushort saturation)
        {
            // Get the latest Bulb color settings (could have change from another source)
            this.LightGet(bulb, true);

            // Update the brightness setting 
            bulb.Saturation = saturation;

            // Create the Packet and send request
            return LightSetHSBK(bulb);
        }
        //--------------------------------------------------------------------

        public ushort? LightGetKelvin(IBulb bulb)
        {
            // Get the latest Bulb HSBK settings (could have change from another source)
            this.LightGet(bulb);
            return bulb.isOffline ? null : (ushort?)bulb.Kelvin;
        }
        //--------------------------------------------------------------------

        public bool LightSetKelvin(IBulb bulb, ushort kelvin)
        {
            // Get the latest Bulb color settings (could have change from another source)
            this.LightGet(bulb, true);

            // Update the brightness setting 
            bulb.Kelvin = kelvin;

            // Create the Packet and send request
            return LightSetHSBK(bulb);
        }
        //--------------------------------------------------------------------

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
        //--------------------------------------------------------------------

		public static void Set(this IBulb bulb, DeviceAcknowledgementResponse r)
		{
		}
        //--------------------------------------------------------------------

		public static void Set(this IBulb bulb, DeviceEchoResponse r)
		{
		}
        //--------------------------------------------------------------------

		public static void Set(this IBulb bulb, DeviceStateGroupResponse r)
		{
			bulb.Group = r.Label;
		}
        //--------------------------------------------------------------------

		public static void Set(this IBulb bulb, DeviceStateHostFirmwareResponse r)
		{
			bulb.HostFirmwareBuild = r.Build;
			bulb.HostFirmwareVersion = r.Version;
		}
        //--------------------------------------------------------------------

		public static void Set(this IBulb bulb, DeviceStateHostInfoResponse r)
		{
			bulb.Signal = r.Signal;
			bulb.TxCount = r.TxCount;
			bulb.RxCount = r.RxCount;
		}
        //--------------------------------------------------------------------

		public static void Set(this IBulb bulb, DeviceStateInfoResponse r)
		{
			bulb.Time = r.Time;
			bulb.Uptime = r.Uptime;
			bulb.Downtime = r.Downtime;
		}
        //--------------------------------------------------------------------

		public static void Set(this IBulb bulb, DeviceStateLabelResponse r)
		{
			bulb.Label = r.Label;
		}
        //--------------------------------------------------------------------

		public static void Set(this IBulb bulb, DeviceStateLocationResponse r)
		{
			bulb.Location = r.Label;
		}
        //--------------------------------------------------------------------

		public static void Set(this IBulb bulb, DeviceStatePowerResponse r)
		{
			bulb.IsOn = r.IsOn;
		}
        //--------------------------------------------------------------------
		
		public static void Set(this IBulb bulb, DeviceStateServiceResponse r)
		{
			bulb.Service = r.Service;
			bulb.Port = r.Port;
		}
        //--------------------------------------------------------------------

		public static void Set(this IBulb bulb, DeviceStateVersionResponse r)
		{
			bulb.Vendor = r.Vendor;
			bulb.Product = (LifxProductEnum)r.Product;
			bulb.Version = r.Version;
		}
        //--------------------------------------------------------------------

		public static void Set(this IBulb bulb, DeviceStateWifiFirmwareResponse r)
		{
			bulb.WifiFirmwareBuild = r.Build;
			bulb.WifiFirmwareVersion = r.Version;
		}
        //--------------------------------------------------------------------

		public static void Set(this IBulb bulb, DeviceStateWifiInfoResponse r)
		{
			bulb.WifiInfoSignal = r.Signal;
			bulb.WifiInfoTxCount = r.TxCount;
			bulb.WifiInfoRxCount = r.RxCount;
		}
        //--------------------------------------------------------------------
		
		public static void Set(this IBulb bulb, LightStatePowerResponse r)
		{
			bulb.IsOn = r.IsOn;
		}
        //--------------------------------------------------------------------

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
        //--------------------------------------------------------------------

		public static void Set(this IBulb bulb, UnknownResponse r)
		{
			throw new ArgumentOutOfRangeException();
		}
        //--------------------------------------------------------------------


	}//class

}//ns
