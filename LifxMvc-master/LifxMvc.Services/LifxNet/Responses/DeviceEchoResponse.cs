using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxNet
{
	public class DeviceEchoResponse : LifxResponseBase
	{
		public DeviceEchoResponse(byte[] payload) : base()
		{
			this.Payload = payload;

		}
		public bool IsOn { get; private set; }

	}
}
