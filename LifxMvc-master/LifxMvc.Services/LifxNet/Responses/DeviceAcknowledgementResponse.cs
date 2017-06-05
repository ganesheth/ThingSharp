using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxNet
{
	/// <summary>
	/// Response to any message sent with ack_required set to 1. 
	/// </summary>
	public class DeviceAcknowledgementResponse : LifxResponseBase
	{
		public DeviceAcknowledgementResponse(byte[] payload) : base() { }
	}
}
