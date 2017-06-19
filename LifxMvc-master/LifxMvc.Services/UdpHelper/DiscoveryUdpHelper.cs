using Eric.Morrison.Logging;
using LifxMvc.Services.Discovery;
using LifxNet;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace LifxMvc.Services.UdpHelper
{
	public class DiscoveryUdpHelper : IDisposable
	{
		class ListenContext
		{
			public int Timeout { get; set; }

			public ListenContext(int expectedCount, int timeout)
			{
				this.Timeout = timeout;
			}
		}

		private const int PORT_NO = 56700;

        private ConcurrentBag<string> foundBulbList = new ConcurrentBag<string>();

        DiscoveryService DiscoverySvc = null;

		Task ListeningTask { get; set; }
        //ManualResetEventSlim StopListeningEvent { get; set; }
        //ManualResetEventSlim StopDiscoveryEvent { get; set; }
        //ManualResetEventSlim IsSafeToSendEvent { get; set; }

        //public event EventHandler<DiscoveryEventArgs> DeviceDiscovered;
		ConcurrentDictionary<string, int> DiscoveryResponses { get; set; }
		DebugLogger Logger { get; set; }

		public DiscoveryUdpHelper()
		{
			this.DiscoveryResponses = new ConcurrentDictionary<string, int>();
            //this.StopListeningEvent = new ManualResetEventSlim(false);
            //this.StopDiscoveryEvent = new ManualResetEventSlim(false);
            //this.IsSafeToSendEvent = new ManualResetEventSlim(true);
			this.Logger = new DebugLogger();
		}

		public void DiscoverBulbs(DiscoveryService ds, LifxPacketBase packet)
		{
            DiscoverySvc = ds;

			Logger.EnterMethod(MethodInfo.GetCurrentMethod(), "{0}", DateTime.Now.ToString("HH:mm:ss.ffff"));
			try
			{
				//var ctx = new ListenContext(timeout);

                this.DiscoverBulbsImpl(packet);

                //var timeoutEvent = new ManualResetEventSlim(false);

                //const int TIMEOUT_NDX = 0;
                //const int SUCCESS_NDX = 1;
                //var waitHandles = new WaitHandle[2];
                //waitHandles[TIMEOUT_NDX] = timeoutEvent.WaitHandle;
                //waitHandles[SUCCESS_NDX] = this.StopDiscoveryEvent.WaitHandle;

                //var which = int.MaxValue;
                //try
                //{
                //    which = WaitHandle.WaitAny(waitHandles, timeout);
                //    this.StopListening();
                //}
                //catch (ThreadAbortException)
                //{ }
			}
			catch (Exception ex)
			{
				Debug.WriteLine(" Exception {0}", ex.Message);
				throw;
			}
			Logger.ExitMethod(MethodInfo.GetCurrentMethod(), "{0}", DateTime.Now.ToString("HH:mm:ss.ffff"));
		}

        void DiscoverBulbsImpl(LifxPacketBase packet)
		{
			Logger.EnterMethod(MethodInfo.GetCurrentMethod(), "{0}", DateTime.Now.ToString("HH:mm:ss.ffff"));

            StartListening();  
            

            //Send(packet, ctx);
            StartSending(packet);
			

            //const int RETRY_INTERVAL = 10000;
            //var wait = new ManualResetEventSlim(false);
            //wait.Wait(RETRY_INTERVAL);
            //this.StopListening();

            //if (this.DiscoveryResponses.Count < ctx.ExpectedCount)
            //  DiscoverBulbsImpl(packet, ctx);

			Logger.ExitMethod(MethodInfo.GetCurrentMethod(), "{0}", DateTime.Now.ToString("HH:mm:ss.ffff"));
		}

        void StartSending(LifxPacketBase packet)
        {
            var listenProc2 = new Thread(this.Send);
            listenProc2.Start(packet);
        }

        public static IPAddress LocalEndpointIpAddress = IPAddress.Any;
        void Send(object ob)
		{
            LifxPacketBase packet = ob as LifxPacketBase;

            // Discovery Loop Settings:
            // Initially, send out a discovery request every 5 seconds until devices are found.
            // Once devices are found, send out a discovery request at a much slow rate to reduce network traffic.
            int waitTime = 5000;
            

            Logger.EnterMethod(MethodInfo.GetCurrentMethod(), "{0}", DateTime.Now.ToString("HH:mm:ss.ffff"));
			//CJKthis.StopListening();
            var data = packet.Serialize();

			
			TraceData(data);
			IPAddress broadcastIP = IPAddress.Broadcast;
			IPEndPoint destEP = new IPEndPoint(broadcastIP, PORT_NO);
			packet.IPEndPoint = destEP;
            //if (!IsSafeToSendEvent.IsSet)
            //    IsSafeToSendEvent.Wait();
            while (true)
            {
                //if (foundBulbList.Count > 1)
                //{
                //    waitTime = 60000;
                //}

                using (UdpClient sender = new UdpClient(new IPEndPoint(LocalEndpointIpAddress, PORT_NO)))// new UdpClient(PORT_NO, AddressFamily.InterNetwork))
                {
                    sender.DontFragment = true;
                    sender.EnableBroadcast = true;

                    //Logger.EnterMethod(MethodInfo.GetCurrentMethod(), "{0}{1}", DateTime.Now.ToString("HH:mm:ss.ffff"), " --- Send Packet Now");
                    Debug.WriteLine("{0}{1}", DateTime.Now.ToString("HH:mm:ss.ffff"), " --- Send Service Discovery Packet");
                    sender.Send(data, data.Length, destEP);
                    packet.TraceSent(sender.Client.LocalEndPoint);

                }
                System.Threading.Thread.Sleep(waitTime);
            }
			Logger.ExitMethod(MethodInfo.GetCurrentMethod(), "{0}", DateTime.Now.ToString("HH:mm:ss.ffff"));
		}
               

		void StartListening()
		{
			Logger.EnterMethod(MethodInfo.GetCurrentMethod(), "{0}", DateTime.Now.ToString("HH:mm:ss.ffff"));
			//this.StopListeningEvent.Reset();

            var listenProc = new Thread(this.Listen);
            listenProc.Start();
            //UDPListener();

            // Give the Listener a little time to get setup before moving on.
            System.Threading.Thread.Sleep(500);

			Logger.ExitMethod(MethodInfo.GetCurrentMethod(), "{0}", DateTime.Now.ToString("HH:mm:ss.ffff"));
		}

        //private void UDPListener()
        //{
        //    Task.Run(() =>
        //    {
        //        using (UdpClient udpClient = new UdpClient(PORT_NO, AddressFamily.InterNetwork))
        //        {
        //            //udpClient.EnableBroadcast = true;
        //            //string loggingEvent = "";
                    
        //            while (true)
        //            {
        //                //IPEndPoint object will allow us to read datagrams sent from any source.
        //                IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);

        //                byte[] receivedResults = udpClient.Receive(ref sender);
        //                //byte[] data = receivedResults;
        //                var response = ResponseFactory.Parse(receivedResults, sender);
        //                //response.TraceReceived(listener.Client.LocalEndPoint);

        //                if (response is DeviceStateServiceResponse && ((LifxNet.DeviceStateServiceResponse)(response)).Service == 1)
        //                {
        //                    var ctx = new DiscoveryContext(sender, response as DeviceStateServiceResponse);

        //                    // If we've already discovered the bulb, then skip it.
        //                    string ip = ctx.Sender.ToString();
        //                    if (foundBulbList.FirstOrDefault(x => x == ip) == null)
        //                    {
        //                        foundBulbList.Add(ip);
        //                        OnBulbDiscovered(ctx);
        //                    }
        //                }
        //            }
        //        }
        //    });
        //}

        //internal void StopListening()
        //{
        //    Logger.EnterMethod(MethodInfo.GetCurrentMethod(), "{0}", DateTime.Now.ToString("HH:mm:ss.ffff"));
        //    this.StopListeningEvent.Set();
        //    Logger.ExitMethod(MethodInfo.GetCurrentMethod(), "{0}", DateTime.Now.ToString("HH:mm:ss.ffff"));
        //}

        void Listen()
        {
            Logger.EnterMethod(MethodInfo.GetCurrentMethod(), "{0}", DateTime.Now.ToString("HH:mm:ss.ffff"));
            //var listenCtx = ob as ListenContext;
            int timeout = 10000;// listenCtx.Timeout;
            //int expectedCount = listenCtx.ExpectedCount;

            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);

            //IsSafeToSendEvent.Reset();
            using (UdpClient listener = new UdpClient(PORT_NO, AddressFamily.InterNetwork))
            {
                try
                {
                    while (true)//!this.StopListeningEvent.IsSet)
                    {
                        var asyncResult = listener.BeginReceive(null, null);
                        //var waitHandles = new WaitHandle[] { asyncResult.AsyncWaitHandle, this.StopListeningEvent.WaitHandle };
                        //var waitHandles = new WaitHandle[] { asyncResult.AsyncWaitHandle };
                        //var which = int.MaxValue;
                        //try
                        //{
                        //    which = WaitHandle.WaitAny(waitHandles, timeout);
                        //}
                        //catch (ThreadAbortException)
                        //{ }
                        //if (0 == which)
                        //{
                        var signaled = asyncResult.AsyncWaitHandle.WaitOne(timeout);
                        if (signaled)
                        {
                            if (asyncResult.IsCompleted)
                            {
                                byte[] data = listener.EndReceive(asyncResult, ref sender);
                                //parse the response.
                                var response = ResponseFactory.Parse(data, sender);
                                response.TraceReceived(listener.Client.LocalEndPoint);

                                if (response is DeviceStateServiceResponse && ((LifxNet.DeviceStateServiceResponse)(response)).Service == 1)
                                {
                                    var ctx = new DiscoveryContext(sender, response as DeviceStateServiceResponse);

                                    // If we've already discovered the bulb, then skip it.
                                    string ip = ctx.Sender.ToString();
                                    if (foundBulbList.FirstOrDefault(x => x == ip) == null)
                                    {
                                        foundBulbList.Add(ip);
                                        //newBulbList.Add(ip); // gets reset when ThingServer requests new found bulbs
                                        OnBulbDiscovered(ctx);
                                    }
                                }
                                else
                                {
                                    //This is a random response broadcast from a bulb.
                                    //Debug.WriteLine(string.Format("{0}: {1}", sender.ToString(), response.ToString()));
                                }
                            }
                        }
                        
                    }
                    Debug.WriteLine("StopListeningEvent.IsSet");
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                    throw;
                }
            }
            //IsSafeToSendEvent.Set();
            Logger.ExitMethod(MethodInfo.GetCurrentMethod(), "{0}", DateTime.Now.ToString("HH:mm:ss.ffff"));
        }

		void OnBulbDiscovered(DiscoveryContext ctx)
		{
            //if (null != this.DeviceDiscovered)
            //{
                var sender = ctx.Sender.ToString();
                Debug.Assert(!string.IsNullOrEmpty(sender));
                DiscoveryResponses[sender] = 0;
                DiscoverySvc.Udp_DeviceDiscovered(this, new DiscoveryEventArgs(ctx));
            //}
            
            //////var action = new Action(() => this.OnBulbDiscovered(ctx));
            //////Task.Factory.StartNew(action);
		}

        ////void OnBulbDiscovered(DiscoveryContext ctx)
        ////{
        ////    Logger.EnterMethod(MethodInfo.GetCurrentMethod(), "{0}", DateTime.Now.ToString("HH:mm:ss.ffff"));
        ////    //publish the response.
        ////    if (null != this.DeviceDiscovered)
        ////    {
        ////        var sender = ctx.Sender.ToString();
        ////        Debug.Assert(!string.IsNullOrEmpty(sender));
        ////        this.DiscoveryResponses[sender] = 0;
        ////        this.DeviceDiscovered(this, new DiscoveryEventArgs(ctx));
        ////        if (ctx.CancelDiscovery)
        ////        {
        ////            this.StopDiscoveryEvent.Set();
        ////        }
        ////    }
        ////    Logger.ExitMethod(MethodInfo.GetCurrentMethod(), "{0}", DateTime.Now.ToString("HH:mm:ss.ffff"));
        ////}

		UdpClient CreateUdpClient()
		{
			var client = new UdpClient(PORT_NO);
			return client;
		}

		static void TraceData(byte[] data)
		{
#if true
			if (null != data)
			{
				//System.Diagnostics.Debug.WriteLine(
				//	string.Join(",", (from a in data select Convert.ToString(a, 2).PadLeft(8, '0')).ToArray()));

				System.Diagnostics.Debug.WriteLine(
					string.Join(",", (from a in data select a.ToString("X2")).ToArray()));
			}

#endif
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
                //if (disposing)
                //{
                //    this.StopListening();
                //}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~DiscoveryUdpHelper() {
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
