using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxNet
{
	public class DeviceStateWifiFirmwareResponse : LifxResponseBase
	{
		public DateTime Build { get; set; }
		public UInt32 Version { get; set; }

		public DeviceStateWifiFirmwareResponse(byte[] payload)
		{
			var nanoseconds = BitConverter.ToUInt64(payload, 0);
			Build = Constants.Epoch.AddMilliseconds(nanoseconds * 0.000001);
			//8..15 UInt64 is reserved
			Version = BitConverter.ToUInt32(payload, 16);
		}
	}
}
