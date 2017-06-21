using LifxMvc.Domain;
using LifxNet.Domain;
using System.Drawing;

namespace LifxMvc.Services
{
	public interface IBulbService
	{
		void DeviceGetGroup(IBulb bulb);
		void DeviceGetLocation(IBulb bulb);
		bool DeviceGetPower(IBulb bulb);
		void DeviceGetVersion(IBulb bulb);
		void DeviceSetPower(IBulb bulb, bool isOn);
		void EchoRequest(IBulb bulb);
		void GetHostFirmware(IBulb bulb);
		void GetHostInfo(IBulb bulb);
		void GetInfo(IBulb bulb);
		void GetLabel(IBulb bulb);
		void GetWifiFirmware(IBulb bulb);
		void GetWifiInfo(IBulb bulb);
		bool Initialize(IBulb bulb);
        bool LightGet(IBulb bulb, bool forceUpdate = false);
        uint? LightGetPower(IBulb bulb);
		//void LightSetColor(IBulb bulb, IHSBK hsbk);
		void LightSetColor(IBulb bulb, Color color);
		void LightSetPower(IBulb bulb, bool power);
		void LightSetWaveform(IBulb bulb, LightSetWaveformCreationContext ctx);
		void SetLabel(IBulb bulb, string label);
	}
}