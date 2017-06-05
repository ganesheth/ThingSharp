using LifxNet;
using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LifxMvc.Services.Discovery
{
	public class DiscoveryContext
	{
		public int ExpectedCount { get; set; }
		public IPEndPoint Sender { get; private set; }
		public DeviceStateServiceResponse Response { get; private set; }
		public bool CancelDiscovery { get; set; }

		public DiscoveryContext(IPEndPoint sender, DeviceStateServiceResponse response, int expectedCount)
		{
			this.Sender = sender;
			this.Response = response;
			this.ExpectedCount = expectedCount;
		}
	}//class
}
