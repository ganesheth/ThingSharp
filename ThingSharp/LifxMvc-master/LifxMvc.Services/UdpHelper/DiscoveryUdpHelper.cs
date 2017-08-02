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
        private const int PORT_NO = 56700; // LIFX Communication port
        private const int SERVICE_PORT_NO = 56701; // Our discovery port
        public static IPAddress LocalEndpointIpAddress = IPAddress.Any;

        const int SEND_DISCOVERY_WAIT = 30000; 
        const int LISTEN_DICOVERY_TIMEOUT = 20000; // 20 seconds

        const int UDP_SERVICE_SUPPORTED = 1;

        bool KEEP_LISTENING = true;
        bool KEEP_RETRYING = true;
        bool KEEP_DISCOVERING = true;

        private UdpClient discoverySocket = new UdpClient();

        private ConcurrentDictionary<string, int> discoveredBulbList = new ConcurrentDictionary<string, int>();

        DiscoveryService DiscoverySvc = null;

        Task ListeningTask { get; set; }
        DebugLogger Logger { get; set; }

		class ListenContext
		{
			public int Timeout { get; set; }

			public ListenContext(int expectedCount, int timeout)
			{
				this.Timeout = timeout;
			}
		}
        //--------------------------------------------------------------------

        private void CreateUdpClient()
        {
            discoverySocket.DontFragment = true;
            discoverySocket.EnableBroadcast = true;

            //sender.ExclusiveAddressUse = false;
            discoverySocket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            discoverySocket.Client.Bind(new IPEndPoint(LocalEndpointIpAddress, SERVICE_PORT_NO));    
        }
        //--------------------------------------------------------------------

        private void CloseUdpClient()
        {
            discoverySocket.Close();
            discoverySocket.Dispose();
        }
        //--------------------------------------------------------------------
        
        public DiscoveryUdpHelper()
		{
			this.Logger = new DebugLogger();
		}
        //--------------------------------------------------------------------

        public void RemoveFromDiscoveredBulbList(string endPoint)
        {
            int value;
            discoveredBulbList.TryRemove(endPoint, out value);
        }
        //--------------------------------------------------------------------

        public void StopDiscoveringBulbs()
        {
            KEEP_LISTENING = false;
            KEEP_RETRYING = false;
            KEEP_DISCOVERING = false;

            CloseUdpClient();
        }
        //--------------------------------------------------------------------
        
        public void DiscoverBulbs(DiscoveryService ds, LifxPacketBase packet)
		{
            DiscoverySvc = ds;

			Logger.EnterMethod(MethodInfo.GetCurrentMethod(), "{0}", DateTime.Now.ToString("HH:mm:ss.ffff"));
			try
			{
                CreateUdpClient();

                // Start the discovery threads
                StartListening();
                StartSending(packet);
			}
			catch (Exception ex)
			{
                Debug.WriteLine(ex.ToString());
				throw;
			}
			Logger.ExitMethod(MethodInfo.GetCurrentMethod(), "{0}", DateTime.Now.ToString("HH:mm:ss.ffff"));
		}
        //--------------------------------------------------------------------

        void StartSending(LifxPacketBase packet)
        {
            var discoverProc = new Thread(this.Send);
            discoverProc.Start(packet);
        }
        //--------------------------------------------------------------------
        
        void Send(object ob)
		{
            // SEND_DISCOVERY_WAIT is used to define how often we will broadcast a discovery request
            
            LifxPacketBase packet = ob as LifxPacketBase;            
            int waitTime = SEND_DISCOVERY_WAIT;

            Logger.EnterMethod(MethodInfo.GetCurrentMethod(), "{0}", DateTime.Now.ToString("HH:mm:ss.ffff"));
            var data = packet.Serialize();
			
			TraceData(data);
			IPAddress broadcastIP = IPAddress.Broadcast;
			IPEndPoint destEP = new IPEndPoint(broadcastIP, PORT_NO);
			packet.IPEndPoint = destEP;
            while (KEEP_DISCOVERING)
            {
                try
                {
                    Debug.WriteLine("{0}{1}", DateTime.Now.ToString("HH:mm:ss.ffff"), " --- Send Service Discovery Packet");
                    discoverySocket.Send(data, data.Length, destEP);
                    packet.TraceSent(discoverySocket.Client.LocalEndPoint);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                    Console.WriteLine("ERROR: {0}", e.Message);
                    StopDiscoveringBulbs();
                    //throw e;
                }                
                
                System.Threading.Thread.Sleep(waitTime);
            }
			Logger.ExitMethod(MethodInfo.GetCurrentMethod(), "{0}", DateTime.Now.ToString("HH:mm:ss.ffff"));
		}
        //--------------------------------------------------------------------

		void StartListening()
		{
			Logger.EnterMethod(MethodInfo.GetCurrentMethod(), "{0}", DateTime.Now.ToString("HH:mm:ss.ffff"));

            var listenProc = new Thread(this.Listen);
            listenProc.Start();

            // Give the Listener a little time to get setup before moving on.
            System.Threading.Thread.Sleep(500);

			Logger.ExitMethod(MethodInfo.GetCurrentMethod(), "{0}", DateTime.Now.ToString("HH:mm:ss.ffff"));
		}
        //--------------------------------------------------------------------

        void Listen()
        {
            Logger.EnterMethod(MethodInfo.GetCurrentMethod(), "{0}", DateTime.Now.ToString("HH:mm:ss.ffff"));
            
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);

            while (KEEP_RETRYING)
            {
                try
                {
                    while (KEEP_LISTENING)
                    {
                        var asyncResult = discoverySocket.BeginReceive(null, null);
                        var signaled = asyncResult.AsyncWaitHandle.WaitOne(LISTEN_DICOVERY_TIMEOUT);
                        if (signaled)
                        {
                            if (asyncResult.IsCompleted)
                            {
                                byte[] data = discoverySocket.EndReceive(asyncResult, ref sender);

                                // parse the response.
                                var response = ResponseFactory.Parse(data, sender);
                                response.TraceReceived(discoverySocket.Client.LocalEndPoint);

                                // Make sure we received the correct message type and Service supported
                                if (response is DeviceStateServiceResponse && ((LifxNet.DeviceStateServiceResponse)(response)).Service == UDP_SERVICE_SUPPORTED)
                                {
                                    string endPoint = sender.Address.ToString();
                                    int value;

                                    // If we've already discovered the bulb, then skip it.
                                    if (!discoveredBulbList.TryGetValue(endPoint, out value))
                                    {
                                        if (discoveredBulbList.TryAdd(endPoint, discoveredBulbList.Count))
                                        {
                                            // Display info so we know we got a response
                                            Console.WriteLine("Received response from: {0}", endPoint);

                                            // Bundle the response and sender info into a single package
                                            var ctx = new DiscoveryContext(sender, response as DeviceStateServiceResponse);

                                            // Fire off a thread to read the bulb properties
                                            OnBulbDiscovered(ctx);
                                        }
                                    }
                                    else
                                    {
                                        //Debug.WriteLine("Ignored {0} -- Already found", sender.Address.ToString(), null);
                                    }
                                }
                                else
                                {
                                    //This is a random response broadcast from a bulb.
                                    Debug.WriteLine("Ignored response {0} from {1}", response.GetType().ToString(), sender.Address.ToString());
                                }
                            }
                        }

                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                    Console.WriteLine("ERROR: {0}", e.Message);
                    StopDiscoveringBulbs();
                    //throw e;

                    // Take a short sleep before checking if the problem is resolved.
                    //System.Threading.Thread.Sleep(2000);
                }
            }

            Logger.ExitMethod(MethodInfo.GetCurrentMethod(), "{0}", DateTime.Now.ToString("HH:mm:ss.ffff"));
        }
        //--------------------------------------------------------------------

		void OnBulbDiscovered(DiscoveryContext ctx)
		{
            var sender = ctx.Sender.ToString();
            Debug.Assert(!string.IsNullOrEmpty(sender));
            DiscoverySvc.Udp_DeviceDiscovered(this, new DiscoveryEventArgs(ctx));
		}
        //--------------------------------------------------------------------

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
        //--------------------------------------------------------------------

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
