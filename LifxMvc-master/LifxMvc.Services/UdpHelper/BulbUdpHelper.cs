using LifxNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace LifxMvc.Services.UdpHelper
{
	public class BulbUdpHelper : IDisposable
	{
		const int MAX_TX_PER_SECOND = 1000 / 10;
		const int MAX_RETRIES = 3;
        const int LOCK_TIMEOUT_15_SECOND = 15000;
        private IAsyncResult _currentAsyncResult;
        private readonly object _objLock = new object();

		bool IsAvailable
		{
			get
			{
				return this.IsAvailableEvent.IsSet;
			}
			set
			{
				if (value)
					this.IsAvailableEvent.Set();
				else
					this.IsAvailableEvent.Reset();
			}
		}

		DateTime LastSentTime { get; set; }
		UdpClient UdpClient { get; set; }
		ManualResetEventSlim StopListeningEvent { get; set; }
		ManualResetEventSlim IsAvailableEvent { get; set; }
        IPEndPoint EndPoint;

		public BulbUdpHelper(IPEndPoint ep)
		{
			this.StopListeningEvent = new ManualResetEventSlim(false);
			this.IsAvailableEvent = new ManualResetEventSlim(true);
			this.UdpClient = this.CreateUdpClient(ep);
            EndPoint = ep;
		}

		UdpClient CreateUdpClient(IPEndPoint ep)
		{
			var client = new UdpClient(ep.Address.ToString(),
				ep.Port);

            client.DontFragment = true;

			return client;
		}

		public void SendAsync(LifxPacketBase packet)
		{
			IsAvailableEvent.Wait();

			var sendImpl = new Action( delegate (){
				this.SendImpl(packet);
			});

			var task = new Task(sendImpl);

			task.ContinueWith((t) => this.IsAvailable = true);
			this.IsAvailable = false;
			task.Start();
		}

        ConcurrentBag<Task> BulbRequestDataTask { get; set; }
        public R Send<R>(LifxPacketBase<R> packet, int retry_count = 0, int timeout = 500)
            where R : LifxResponseBase
        {

            R result = null;
            LifxResponseBase response = null;

            bool enteredLock = false;

            try
            {
                enteredLock = Monitor.TryEnter(_objLock, LOCK_TIMEOUT_15_SECOND);

                if(enteredLock)
                {   
                    // Start the 'GetResponse' method in a Task thread (this starts the response lisenter
                    //  before we send the packet)
                    var action = new Action(() => response = GetResponse(packet.Header.Source, timeout));
                    var task = Task.Factory.StartNew(action);

                    // Send the data packet to the bulb
                    byte[] data = this.SendImpl(packet);

                    // wait for the response from the bulb to be read before continuing
                    Task.WaitAll(task);

                    //this.UdpClient.Close();// Dispose();
                    this.UdpClient.Dispose();
                    this.UdpClient = this.CreateUdpClient(EndPoint);
                }
            }
            finally
            {
                if(enteredLock)
                    Monitor.Exit(_objLock);
            }

            if (response is R)
            {
                result = response as R;
            }
            else
            {
                // If the response is NULL, then retry sending the data again and increase
                // the listener wait timeout.
                if (MAX_RETRIES > retry_count)
                {
                    packet.Header.Source += 100;
                    result = this.Send(packet, ++retry_count, (timeout * 2));
                }
            }

            return result;
        }

		byte[] SendImpl(LifxPacketBase packet)
		{
			byte[] result = null;
			try
			{
				var data = packet.Serialize();

				TraceData(data);

				this.Throttle();
				var sent = this.UdpClient.Send(data, data.Length);
				this.LastSentTime = DateTime.Now;
				packet.TraceSent(this.UdpClient.Client.LocalEndPoint);

				Debug.Assert(sent == data.Length);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(" Exception {0}", ex.Message);
				throw;
			}
			return result;
		}

		void Throttle()
		{
            // We throttle the send requests to the bulb because Lifx can't exceed 20 requests per second
			var ts = DateTime.Now - this.LastSentTime;
			if (ts.Milliseconds < MAX_TX_PER_SECOND)
			{
				var wait = new ManualResetEventSlim(false);
				wait.Wait(ts.Milliseconds);
			}
		}        

		LifxResponseBase GetResponse(uint frameSource, int timeout)
		{
            byte[] data = null;
			LifxResponseBase result = null;

            uint responseSource = 0;
            byte responseSequence = 0;

            try
            {
                while (responseSource < frameSource)//Compare sources in order to match the packet to the response.
                {
                    result = null;
                    var asyncResult = this.UdpClient.BeginReceive(null, null);
                    _currentAsyncResult = asyncResult;

                    var signaled = asyncResult.AsyncWaitHandle.WaitOne(timeout);
                    if (signaled)
                    {
                        if (_currentAsyncResult == asyncResult)
                        {
                            if (asyncResult.IsCompleted)
                            {
                                //IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                                //data = this.UdpClient.EndReceive(asyncResult, ref sender);

                                data = this.UdpClient.EndReceive(asyncResult, ref EndPoint);
                                TraceData(data);

                                result = ResponseFactory.Parse(data, EndPoint);
                                responseSource = result.Source;
                                responseSequence = result.Sequence;
                                result.TraceReceived(this.UdpClient.Client.LocalEndPoint);
                            }
                        }
                        else
                        {
                            Debug.WriteLine("{0}{1}", DateTime.Now.ToString("HH:mm:ss.ffff"), " --- UDP BeginReceive and EndReceive don't match up.");
                            break;
                        }
                    }
                    else
                    {
                        // We've timed out.
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                //throw;
            }

			return result;
		}        

		static void TraceData(byte[] data)
		{
#if false
			if (null != data)
			{
				System.Diagnostics.Debug.WriteLine(
					string.Join(",", (from a in data select Convert.ToString(a, 2).PadLeft(8, '0')).ToArray()));

				//System.Diagnostics.Debug.WriteLine(
				//	string.Join(",", (from a in data select a.ToString("X2")).ToArray()));
			}

#endif
		}

		public void Dispose()
		{
            this.UdpClient.Close();
			this.UdpClient.Dispose();
		}
	}//class 

}//ns
