using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxNet
{
	public class DeviceStateInfoResponse : LifxResponseBase
	{
		public DateTime Time { get; set; }
		public DateTime Uptime { get; set; }
		public DateTime Downtime { get; set; }
		public DeviceStateInfoResponse(byte[] payload)
		{
			var nanoseconds = BitConverter.ToUInt64(payload, 0);
			this.Time = Constants.Epoch.AddMilliseconds(nanoseconds * 0.000001);

			nanoseconds = BitConverter.ToUInt64(payload, 0);
			this.Uptime = Constants.Epoch.AddMilliseconds(nanoseconds * 0.000001);

			nanoseconds = BitConverter.ToUInt64(payload, 0);
			this.Downtime = Constants.Epoch.AddMilliseconds(nanoseconds * 0.000001);

		}
	}
}
