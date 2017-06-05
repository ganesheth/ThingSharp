using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxNet
{
	public class DeviceStateHostFirmwareResponse : LifxResponseBase
	{
		/// <summary>
		/// Firmware build time
		/// </summary>
		public DateTime Build { get; private set; }
		/// <summary>
		/// Firmware version
		/// </summary>
		public UInt32 Version { get; private set; }
		public DeviceStateHostFirmwareResponse(byte[] payload) : base()
		{
			var nanoseconds = BitConverter.ToUInt64(payload, 0);
			Build = Constants.Epoch.AddMilliseconds(nanoseconds * 0.000001);
			//8..15 UInt64 is reserved
			Version = BitConverter.ToUInt32(payload, 16);
		}
	}
}
