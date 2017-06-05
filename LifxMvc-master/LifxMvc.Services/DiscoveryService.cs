using LifxMvc.Domain;
using LifxMvc.Services.Discovery;
using LifxMvc.Services.UdpHelper;
using LifxNet;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LifxMvc.Services
{
	public class DiscoveryService : IDisposable, IDiscoveryService
	{
		ConcurrentBag<IBulb> Bulbs { get; set; }
		ManualResetEventSlim Wait { get; set; }



		public List<IBulb> DiscoverAsync(int expectedCount)
		{
			this.Bulbs = new ConcurrentBag<IBulb>();
			this.BulbInitializationTasks = new ConcurrentBag<Task>();
			var packet = new DeviceGetServicePacket();

			var udp = UdpHelperManager.Instance.DiscoveryUdpHelper;
			udp.DeviceDiscovered += Udp_DeviceDiscovered;

			//this.Wait = new ManualResetEventSlim(false);
			const int TIMEOUT = 2 * 1000;
			udp.DiscoverBulbs(packet, expectedCount, TIMEOUT);
			//var success = this.Wait.Wait(TIMEOUT);

			this._sw = Stopwatch.StartNew();
			var taskArr = this.BulbInitializationTasks.ToArray();
			Task.WaitAll(taskArr);
			_sw.Stop();
			Debug.WriteLine("DiscoveryService: DiscoverAsync " + _sw.Elapsed);

			//Debug.Assert(Bulbs.Count == expectedCount);
			var result = this.Bulbs.ToList();

			return result;
		}

		Stopwatch _sw;
		ConcurrentBag<Task> BulbInitializationTasks { get; set; }
		private void Udp_DeviceDiscovered(object sender, DiscoveryEventArgs e)
		{
			var action = new Action(()=> this.Udp_DeviceDiscoveredAsync(sender, e));
			var task = Task.Factory.StartNew(action);
			BulbInitializationTasks.Add(task);

		}

		private void Udp_DeviceDiscoveredAsync(object sender, DiscoveryEventArgs e)
		{
			var ctx = e.DiscoveryContext;
			if (null == this.Bulbs.FirstOrDefault(x => x.IPEndPoint.ToString() == ctx.Sender.ToString()))
			{
				var bulb = new Bulb()
				{
					IPEndPoint = ctx.Sender,
					Service = ctx.Response.Service,
					Port = ctx.Response.Port,
					TargetMacAddress = ctx.Response.TargetMacAddress,
					LastSeen = DateTime.UtcNow
				};

				this.Bulbs.Add(bulb);
				var bulbSvc = new BulbService();
				bulbSvc.Initialize(bulb);

				Debug.WriteLine(Bulbs.Count);
				if (this.Bulbs.Count == ctx.ExpectedCount)
				{
					var udp = sender as DiscoveryUdpHelper;
					udp.DeviceDiscovered -= this.Udp_DeviceDiscovered;
					ctx.CancelDiscovery = true;
					//this.Wait.Set();
				}
			}
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
		// ~DiscoveryService() {
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
