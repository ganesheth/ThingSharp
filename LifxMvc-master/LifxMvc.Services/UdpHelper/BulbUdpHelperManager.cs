using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LifxMvc.Services.UdpHelper
{
	public class UdpHelperManager : IDisposable
	{
		static public UdpHelperManager Instance { get; private set; }
		ConcurrentDictionary<string, BulbUdpHelper> Dictionary { get; set; }

		DiscoveryUdpHelper _discoveryUdpHelper;
		public DiscoveryUdpHelper DiscoveryUdpHelper
		{
			get
			{
				if (null == _discoveryUdpHelper)
					_discoveryUdpHelper = new DiscoveryUdpHelper();
				return _discoveryUdpHelper;
			}
		}

		static UdpHelperManager()
		{
			Instance = new UdpHelperManager();
		}

		UdpHelperManager()
		{
			this.Dictionary = new ConcurrentDictionary<string , BulbUdpHelper>();
		}

		public BulbUdpHelper this[IPEndPoint key]
		{
			get { return this.Lookup(key); }
		}

		BulbUdpHelper Lookup(IPEndPoint endPoint)
		{
			BulbUdpHelper result = null;
			var key = endPoint.Address.ToString();
			if (this.Dictionary.ContainsKey(key))
			{
				result = this.Dictionary[key];
			}
			else
			{
				result = new BulbUdpHelper(endPoint);
				Debug.Assert(null != result);
				Debug.Assert(null != key);
				Debug.Assert(null != this.Dictionary);
				this.Dictionary[key] = result;
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
					var list = this.Dictionary.Values.ToList();
					list.ForEach(x => x.Dispose());
					if (null != _discoveryUdpHelper)
						_discoveryUdpHelper.Dispose();
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~BulbUdpHelperManager() {
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

	}//class
}//ns
