using LifxMvc.Domain;
using LifxNet.Domain;
using System;
using System.Drawing;

namespace LifxMvc.Services
{
	public class BulbServiceMock : IBulbService
	{
		public void DeviceGetGroup(IBulb bulb)
		{
			throw new NotImplementedException();
		}

		public void DeviceGetLocation(IBulb bulb)
		{
			throw new NotImplementedException();
		}

		public bool DeviceGetPower(IBulb bulb)
		{
			throw new NotImplementedException();
		}

		public void DeviceGetVersion(IBulb bulb)
		{
			throw new NotImplementedException();
		}

		public void DeviceSetPower(IBulb bulb, bool isOn)
		{
			throw new NotImplementedException();
		}

		public void EchoRequest(IBulb bulb)
		{
			throw new NotImplementedException();
		}

		public void GetHostFirmware(IBulb bulb)
		{
			throw new NotImplementedException();
		}

		public void GetHostInfo(IBulb bulb)
		{
			throw new NotImplementedException();
		}

		public void GetInfo(IBulb bulb)
		{
			throw new NotImplementedException();
		}

		public void GetLabel(IBulb bulb)
		{
			throw new NotImplementedException();
		}

		public void GetWifiFirmware(IBulb bulb)
		{
			throw new NotImplementedException();
		}

		public void GetWifiInfo(IBulb bulb)
		{
			throw new NotImplementedException();
		}

		public void Initialize(IBulb bulb)
		{
			throw new NotImplementedException();
		}

        public void LightGet(IBulb bulb, bool forceUpdate = false)
		{
			throw new NotImplementedException();
		}

		public bool LightGetPower(IBulb bulb)
		{
			throw new NotImplementedException();
		}

		public void LightSetColor(IBulb bulb, IHSBK hsbk)
		{
			throw new NotImplementedException();
		}

		public void LightSetPower(IBulb bulb, bool power)
		{
			throw new NotImplementedException();
		}

		public void LightSetWaveform(IBulb bulb, LightSetWaveformCreationContext ctx)
		{
			throw new NotImplementedException();
		}

		public void SetLabel(IBulb bulb, string label)
		{
			throw new NotImplementedException();
		}

		public void LightSetColor(IBulb bulb, Color color)
		{
		}

	}
}
