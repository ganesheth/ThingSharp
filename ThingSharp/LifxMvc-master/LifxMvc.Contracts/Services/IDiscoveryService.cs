using System.Collections.Generic;
using LifxMvc.Domain;
using System;

namespace LifxMvc.Services
{
	public interface IDiscoveryService : IDisposable
	{
		void DiscoverAsync();
	}
}