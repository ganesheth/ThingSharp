using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LifxMvc.Domain;
using LifxMvc.Domain.Mocks;

namespace LifxMvc.Services
{
	public class DiscoveryServiceMock : IDiscoveryService, IDisposable
	{
		public List<IBulb> DiscoverAsync(int expectedCount)
		{
			return CreateBulbs();
		}

		List<IBulb> CreateBulbs()
		{
			var result = new List<IBulb>();
			for (int groupNo = 1; groupNo < 5; groupNo++)
			{
				for (int bulbNo = 1; bulbNo < 3; bulbNo++)
				{
					var bulb = new BulbMock();
					bulb.Brightness = 100;
					bulb.Hue = 100;
					bulb.Saturation = 100;
					bulb.Kelvin = 100;
					bulb.Label = string.Format("Bulb{0}", bulbNo);
					bulb.Group = string.Format("Group{0}", groupNo);
					bulb.IsOn = false;
					result.Add(bulb);
				}
			}


			return result;
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~DiscoveryServiceMock() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}
		#endregion
	}
}
