using System;

namespace LifxMvc.Services.Discovery
{

	public class DiscoveryEventArgs : EventArgs
	{
		public DiscoveryContext DiscoveryContext { get; private set; }
		public DiscoveryEventArgs(DiscoveryContext discoveryContext)
		{
			this.DiscoveryContext = discoveryContext;
		}
	}

}
