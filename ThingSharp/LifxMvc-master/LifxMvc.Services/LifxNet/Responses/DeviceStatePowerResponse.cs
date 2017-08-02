using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxNet
{
	public class DeviceStatePowerResponse : LifxResponseBase
	{
		public bool IsOn { get; private set; }
		public DeviceStatePowerResponse(byte[] payload) : base()
		{
			IsOn = BitConverter.ToUInt16(payload, 0) > 0;
		}

	}
}
