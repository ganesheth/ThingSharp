using System.Collections.Generic;
using LifxMvc.Domain;
using System;

namespace LifxMvc.Services
{
	public interface IDiscoveryService : IDisposable
	{
		List<IBulb> DiscoverAsync(int expectedCount);
	}
}