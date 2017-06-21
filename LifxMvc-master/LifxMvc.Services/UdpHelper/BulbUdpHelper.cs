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
		const int MAX_RETRIES = 2;

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
            
            this.UdpClient.Close();
            this.UdpClient = this.CreateUdpClient(EndPoint);

			R result = null;

            BulbRequestDataTask = new ConcurrentBag<Task>();
            LifxResponseBase response = null;
            var action = new Action(() => response = GetResponse(packet.Header.Source, timeout));
            var task = Task.Factory.StartNew(action);
            BulbRequestDataTask.Add(task);

			byte[] data = this.SendImpl(packet);
            //var response = this.GetResponse(packet.Header.Source);

            var taskArr = BulbRequestDataTask.ToArray();
            Task.WaitAll(taskArr);

			//var response = this.GetResponse(packet.Header.Source);
			if (response is R)
			{
				result = response as R;
			}
			else 
			{
                if (MAX_RETRIES > retry_count)
				{//Recursing we will go, recursing we will go, hi, ho, the merry-oh, ....
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
			var ts = DateTime.Now - this.LastSentTime;
			if (ts.Milliseconds < MAX_TX_PER_SECOND)
			{
				var wait = new ManualResetEventSlim(false);
				wait.Wait(ts.Milliseconds);
			}
		}        

		LifxResponseBase GetResponse(uint frameSource, int timeout)
		{
            //const int TIMEOUT = 1500; //cjk
            byte[] data = null;
			LifxResponseBase result = null;

            uint responseSource = 0;
            byte responseSequence = 0;

            while (responseSource < frameSource)//Compare sources in order to match the packet to the response.
            {
                result = null;
                var asyncResult = this.UdpClient.BeginReceive(null, null);

                var signaled = asyncResult.AsyncWaitHandle.WaitOne(timeout);
                if (signaled)
                {
                    if (asyncResult.IsCompleted)
                    {
                        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                        data = this.UdpClient.EndReceive(asyncResult, ref sender);
                        TraceData(data);

                        result = ResponseFactory.Parse(data, sender);
                        responseSource = result.Source;
                        responseSequence = result.Sequence;
                        result.TraceReceived(this.UdpClient.Client.LocalEndPoint);
                    }
                }
                else
                {// We've timed out.
                    break;
                }
            }

			return result;
		}

        ////LifxResponseBase GetResponse2(uint frameSource)
        ////{
        ////    //const int TIMEOUT = 5; //cjk
        ////    //byte[] data = null;
        ////    LifxResponseBase result = null;

        ////    uint responseSource = 0;
        ////    byte responseSequence = 0;

        ////    while (responseSource < frameSource)//Compare sources in order to match the packet to the response.
        ////    {
        ////        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        ////        byte[] receivedData = UdpClient.Receive(ref sender);
        ////        TraceData(receivedData);

        ////        result = ResponseFactory.Parse(receivedData, sender);
        ////        responseSource = result.Source;
        ////        responseSequence = result.Sequence;
        ////        result.TraceReceived(this.UdpClient.Client.LocalEndPoint);
        ////    }
        ////    return result;
        ////}

        ////static void test(IAsyncResult asyncResult)
        ////{
        ////    UdpClient client = (UdpClient)asyncResult.AsyncState;
        ////    try
        ////    {
        ////        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        ////        byte[] result = null;
        ////        result = client.EndReceive(asyncResult, ref sender);

        ////        TraceData(result);
        ////    }
        ////    catch (SocketException)
        ////    {
        ////        throw;
        ////    }
        ////}

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
