using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxNet
{
	public class DeviceStateLabelResponse : LifxResponseBase
	{
		public string Label { get; private set; }
		public DeviceStateLabelResponse(byte[] payload) : base()
		{

			if (payload != null)
				this.Label = Encoding.UTF8.GetString(payload, 0, payload.Length).Replace("\0", "").Trim();
		}
	}
}
