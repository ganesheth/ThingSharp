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
        ConcurrentBag<IBulb> NewBulbList { get; set; }
		ManualResetEventSlim Wait { get; set; }

        DiscoveryUdpHelper _udp = null;

        Stopwatch _sw;

        public List<IBulb> GetDiscoveredBulbs()
        {
            List<IBulb> bulbList = new List<IBulb>();

            // Copy the bulb list send back because we are going to clear out the new bulb list before returning
            foreach (IBulb b in NewBulbList)
            {
                bulbList.Add(b);                
            }

            // Now that we got the new bulbs, clear the list
            IBulb item;
            while (NewBulbList.TryTake(out item)) ;

            return bulbList;
        }

		public void DiscoverAsync()
		{
			Bulbs = new ConcurrentBag<IBulb>();
            NewBulbList = new ConcurrentBag<IBulb>();
            //BulbInitializationTasks = new ConcurrentBag<Task>();
            var packet = new DeviceGetServicePacket();

            _udp = UdpHelperManager.Instance.DiscoveryUdpHelper;
            //_udp.DeviceDiscovered += Udp_DeviceDiscovered;

            //this.Wait = new ManualResetEventSlim(false);
            //const int TIMEOUT = 2 * 1000;
            _udp.DiscoverBulbs(this, packet);
            //var success = this.Wait.Wait(TIMEOUT);
                                    
            //_sw = Stopwatch.StartNew();
            //var taskArr = BulbInitializationTasks.ToArray();
			////////////////////////////////////////////////////////////////////////////////////CJKTask.WaitAll(taskArr);
            //_sw.Stop();
            //Debug.WriteLine("DiscoveryService: DiscoverAsync " + _sw.Elapsed);

			//Debug.Assert(Bulbs.Count == expectedCount);
            //var result = Bulbs.ToList();

            //return result;
		}

		
        //ConcurrentBag<Task> BulbInitializationTasks { get; set; }
		public void Udp_DeviceDiscovered(object sender, DiscoveryEventArgs e)
		{
            //var ctx = e.DiscoveryContext;
            //if (null == Bulbs.FirstOrDefault(x => x.IPEndPoint.ToString() == ctx.Sender.ToString()))
            //{
                var action = new Action(() => Udp_DeviceDiscoveredAsync(sender, e));
                var task = Task.Factory.StartNew(action);
                //BulbInitializationTasks.Add(task);
            //}
		}

        private void Udp_DeviceDiscoveredAsync(object sender, DiscoveryEventArgs e)
        {
            var ctx = e.DiscoveryContext;

            var bulb = new Bulb()
            {
                IPEndPoint = ctx.Sender,
                Service = ctx.Response.Service,
                Port = ctx.Response.Port,
                TargetMacAddress = ctx.Response.TargetMacAddress,
                LastSeen = DateTime.UtcNow
            };

            //Bulbs.Add(bulb);
            var bulbSvc = new BulbService();
            bulbSvc.Initialize(bulb);

            //int retries = 0;
            //while (string.IsNullOrEmpty(bulb.Label) && retries++ < 5)
            //{
            //    bulbSvc.Initialize(bulb);
            //}

            Bulbs.Add(bulb);
            NewBulbList.Add(bulb); // gets reset when ThingServer requests new found bulbs

            Debug.WriteLine(Bulbs.Count);
            //if (Bulbs.Count == ctx.ExpectedCount)
            //{
            //    var udp = sender as DiscoveryUdpHelper;
            //    udp.DeviceDiscovered -= Udp_DeviceDiscovered;
            //    ctx.CancelDiscovery = true;
            //    //this.Wait.Set();
            //}
            //}
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
