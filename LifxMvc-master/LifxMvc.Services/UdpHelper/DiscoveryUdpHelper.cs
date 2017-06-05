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

namespace LifxMvc.Services.UdpHelper
{
	public class DiscoveryUdpHelper : IDisposable
	{
		class ListenContext
		{
			public int ExpectedCount { get; set; }
			public int Timeout { get; set; }

			public ListenContext(int expectedCount, int timeout)
			{
				this.ExpectedCount = expectedCount;
				this.Timeout = timeout;
			}
		}

		private const int PORT_NO = 56700;

		Task ListeningTask { get; set; }
		ManualResetEventSlim StopListeningEvent { get; set; }
		ManualResetEventSlim StopDiscoveryEvent { get; set; }
		ManualResetEventSlim IsSafeToSendEvent { get; set; }

		public event EventHandler<DiscoveryEventArgs> DeviceDiscovered;
		ConcurrentDictionary<string, int> DiscoveryResponses { get; set; }
		DebugLogger Logger { get; set; }

		public DiscoveryUdpHelper()
		{
			this.DiscoveryResponses = new ConcurrentDictionary<string, int>();
			this.StopListeningEvent = new ManualResetEventSlim(false);
			this.StopDiscoveryEvent = new ManualResetEventSlim(false);
			this.IsSafeToSendEvent = new ManualResetEventSlim(true);
			this.Logger = new DebugLogger();
		}

		public void DiscoverBulbs(LifxPacketBase packet, int expectedCount, int timeout)
		{
			Logger.EnterMethod(MethodInfo.GetCurrentMethod(), "{0}", DateTime.Now.ToString("HH:mm:ss.ffff"));
			try
			{
				var ctx = new ListenContext(expectedCount, timeout);

				this.DiscoverBulbsImpl(packet, ctx);

				var timeoutEvent = new ManualResetEventSlim(false);

				const int TIMEOUT_NDX = 0;
				const int SUCCESS_NDX = 1;
				var waitHandles = new WaitHandle[2];
				waitHandles[TIMEOUT_NDX] = timeoutEvent.WaitHandle;
				waitHandles[SUCCESS_NDX] = this.StopDiscoveryEvent.WaitHandle;

				var which = int.MaxValue;
				try
				{
					which = WaitHandle.WaitAny(waitHandles, timeout);
					this.StopListening();
				}
				catch (ThreadAbortException)
				{ }
			}
			catch (Exception ex)
			{
				Debug.WriteLine(" Exception {0}", ex.Message);
				throw;
			}
			Logger.ExitMethod(MethodInfo.GetCurrentMethod(), "{0}", DateTime.Now.ToString("HH:mm:ss.ffff"));
		}

		void DiscoverBulbsImpl(LifxPacketBase packet, ListenContext ctx)
		{
			Logger.EnterMethod(MethodInfo.GetCurrentMethod(), "{0}", DateTime.Now.ToString("HH:mm:ss.ffff"));
			this.Send(packet);
			this.StartListening(ctx);

			const int RETRY_INTERVAL = 2000;
			var wait = new ManualResetEventSlim(false);
			wait.Wait(RETRY_INTERVAL);
			this.StopListening();

			if (this.DiscoveryResponses.Count < ctx.ExpectedCount)
				DiscoverBulbsImpl(packet, ctx);

			Logger.ExitMethod(MethodInfo.GetCurrentMethod(), "{0}", DateTime.Now.ToString("HH:mm:ss.ffff"));
		}

        public static IPAddress LocalEndpointIpAddress = IPAddress.Any;
		void Send(LifxPacketBase packet)
		{
			Logger.EnterMethod(MethodInfo.GetCurrentMethod(), "{0}", DateTime.Now.ToString("HH:mm:ss.ffff"));
			this.StopListening();
			var data = packet.Serialize();
			//TraceData(data);
			IPAddress broadcastIP = IPAddress.Broadcast;
			IPEndPoint destEP = new IPEndPoint(broadcastIP, PORT_NO);
			packet.IPEndPoint = destEP;
			if (!IsSafeToSendEvent.IsSet)
				IsSafeToSendEvent.Wait();
            using (UdpClient sender = new UdpClient(new IPEndPoint(LocalEndpointIpAddress, PORT_NO)))// new UdpClient(PORT_NO, AddressFamily.InterNetwork))
            {
                sender.DontFragment = true;
                sender.EnableBroadcast = true;

                //var resendWait = new ManualResetEventSlim(false);
                sender.Send(data, data.Length, destEP);
                packet.TraceSent(sender.Client.LocalEndPoint);
                //resendWait.Wait(100);

                //sender.Send(data, data.Length, destEP);
                ////resendWait.Wait(100);

                //sender.Send(data, data.Length, destEP);
                ////resendWait.Wait(100);

            }
			Logger.ExitMethod(MethodInfo.GetCurrentMethod(), "{0}", DateTime.Now.ToString("HH:mm:ss.ffff"));
		}

		void StartListening(ListenContext ctx)
		{
			Logger.EnterMethod(MethodInfo.GetCurrentMethod(), "{0}", DateTime.Now.ToString("HH:mm:ss.ffff"));
			this.StopListeningEvent.Reset();

			var listenProc = new Thread(this.Listen);
			listenProc.Start(ctx);
			Logger.ExitMethod(MethodInfo.GetCurrentMethod(), "{0}", DateTime.Now.ToString("HH:mm:ss.ffff"));
		}

		internal void StopListening()
		{
			Logger.EnterMethod(MethodInfo.GetCurrentMethod(), "{0}", DateTime.Now.ToString("HH:mm:ss.ffff"));
			this.StopListeningEvent.Set();
			Logger.ExitMethod(MethodInfo.GetCurrentMethod(), "{0}", DateTime.Now.ToString("HH:mm:ss.ffff"));
		}

		void Listen(object ob)
		{
			Logger.EnterMethod(MethodInfo.GetCurrentMethod(), "{0}", DateTime.Now.ToString("HH:mm:ss.ffff"));
			var listenCtx = ob as ListenContext;
			int timeout = listenCtx.Timeout;
			int expectedCount = listenCtx.ExpectedCount;

			IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);

			IsSafeToSendEvent.Reset();
			using (UdpClient listener = new UdpClient(PORT_NO, AddressFamily.InterNetwork))
			{
				listener.EnableBroadcast = true;
				try
				{
					while (!this.StopListeningEvent.IsSet)
					{
						var asyncResult = listener.BeginReceive(null, null);
						var waitHandles = new WaitHandle[] { asyncResult.AsyncWaitHandle, this.StopListeningEvent.WaitHandle };
						var which = int.MaxValue;
						try
						{
							which = WaitHandle.WaitAny(waitHandles, timeout);
						}
						catch (ThreadAbortException)
						{ }
						if (0 == which)
						{
							if (asyncResult.IsCompleted)
							{
								byte[] data = listener.EndReceive(asyncResult, ref sender);
								//parse the response.
								var response = ResponseFactory.Parse(data, sender);
								response.TraceReceived(listener.Client.LocalEndPoint);

								if (response is DeviceStateServiceResponse)
								{
									var ctx = new DiscoveryContext(sender, response as DeviceStateServiceResponse, expectedCount);
									this.OnBulbDiscoveredAsync(ctx);
								}
								else 
								{
									//This is a random response broadcast from a bulb.
									//Debug.WriteLine(string.Format("{0}: {1}", sender.ToString(), response.ToString()));
								}
							}
						}
						else if (1 == which)
						{
							listener.Close();
							this.StopListening();
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
			IsSafeToSendEvent.Set();
			Logger.ExitMethod(MethodInfo.GetCurrentMethod(), "{0}", DateTime.Now.ToString("HH:mm:ss.ffff"));
		}

		void OnBulbDiscoveredAsync(DiscoveryContext ctx)
		{
			var action = new Action(() => this.OnBulbDiscovered(ctx));
			Task.Factory.StartNew(action);
		}

		void OnBulbDiscovered(DiscoveryContext ctx)
		{
			Logger.EnterMethod(MethodInfo.GetCurrentMethod(), "{0}", DateTime.Now.ToString("HH:mm:ss.ffff"));
			//publish the response.
			if (null != this.DeviceDiscovered)
			{
				var sender = ctx.Sender.ToString();
				Debug.Assert(!string.IsNullOrEmpty(sender));
				this.DiscoveryResponses[sender] = 0;
				this.DeviceDiscovered(this, new DiscoveryEventArgs(ctx));
				if (ctx.CancelDiscovery)
				{
					this.StopDiscoveryEvent.Set();
				}
			}
			Logger.ExitMethod(MethodInfo.GetCurrentMethod(), "{0}", DateTime.Now.ToString("HH:mm:ss.ffff"));
		}

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
				if (disposing)
				{
					this.StopListening();
				}

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
