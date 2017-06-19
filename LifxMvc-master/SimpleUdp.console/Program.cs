/* 
* The purpose of this program is to provide a minimal example of using UDP to 
* send data.
* It transmits broadcast packets and displays the text in a console window.
* This was created to work with the program UDP_Minimum_Listener.
* Run both programs, send data with Talker, receive the data with Listener.
*/
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Eric.Morrison
{
    class Program
    {

        static void Main(string[] args)
        {
            new Program().MainImpl(args);
        }

        void MainImpl(string[] args)
        {
            var waitEvent = new AutoResetEvent(false);
            var done = false;

            var udpClient = new UDPListener();

            while (!done)
            {
                Console.WriteLine("Enter text to send, blank line to quit");
                string text_to_send = Console.ReadLine();
                if (text_to_send.Length == 0)
                {
                    done = true;
                    udpClient.StopListening();
                }
                else
                {
                    udpClient.BroadcastAndListen("this is a test.", this.ListenCallback);

                    waitEvent.WaitOne(10 * 1000);

                }
            } // end of while (!done)
        } // end of main()


        void ListenCallback()
        {
        }

    } // end of class Program




    public class UDPListener
    {
        const string BROADCAST_IP_ADDRESS = "192.168.1.255";
        const int PORT_NO = 11000;
        private const string Port = "56700";

        Task ListeningTask { get; set; }
        private const int _listenPort = 11000;

        ManualResetEventSlim StopListeningEvent { get; set; }

        public UDPListener()
        {
            this.StopListeningEvent = new ManualResetEventSlim(false);
        }

        public void StartListening(Action listenCallback)
        {
            this.StopListeningEvent.Reset();
            var action = new Action(()=> Listen(listenCallback));
            this.ListeningTask = new Task(action);
            this.ListeningTask.Start();
        }

        internal void StopListening()
        {
            this.StopListeningEvent.Set();
        }

        public void Listen(Action listenCallback)
        {
            const int TIMEOUT = 100;
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);

            using (UdpClient listener = new UdpClient(_listenPort))
            {
                try
                {
                    while (!StopListeningEvent.IsSet)
                    {
                        var asyncResult = listener.BeginReceive(null, null);
                        var waitHandles = new WaitHandle[] { asyncResult.AsyncWaitHandle, this.StopListeningEvent.WaitHandle };
                        var which = WaitHandle.WaitAny(waitHandles, TIMEOUT);
                        if (0 == which)
                        {
                            if (asyncResult.IsCompleted)
                            {
                                byte[] data = listener.EndReceive(asyncResult, ref sender);
                                //parse the response.
                                //publish the response.
                                string received_data = Encoding.ASCII.GetString(data, 0, data.Length);
                                if (null != listenCallback)
                                {
                                    listenCallback();
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
        }

        void ProcessMessage(byte[] data)
        {

        }

        public void BroadcastAndListen(string message, Action listenCallback)
        {
            this.StartListening(listenCallback);

            Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            #region comments 
            // create an address object and populate it with the IP address that we will use
            // in sending at data to. This particular address ends in 255 meaning we will send
            // to all devices whose address begins with 192.168.2.
            // However, objects of class IPAddress have other properties. In particular, the
            // property AddressFamily. Does this constructor examine the IP address being
            // passed in, determine that this is IPv4 and set the field. If so, the notes
            // in the help file should say so.
            #endregion
            IPAddress send_to_address = IPAddress.Parse(BROADCAST_IP_ADDRESS);
            #region comments
            // IPEndPoint appears (to me) to be a class defining the first or final data
            // object in the process of sending or receiving a communications packet. It
            // holds the address to send to or receive from and the port to be used. We create
            // this one using the address just built in the previous line, and adding in the
            // port number. As this will be a broadcase message, I don't know what role the
            // port number plays in this.
            #endregion
            IPEndPoint sending_end_point = new IPEndPoint(send_to_address, PORT_NO);
            #region comments
            // The below three lines of code will not work. They appear to load
            // the variable broadcast_string witha broadcast address. But that
            // address causes an exception when performing the send.
            //
            //string broadcast_string = IPAddress.Broadcast.ToString();
            //Console.WriteLine("broadcast_string contains {0}", broadcast_string);
            //send_to_address = IPAddress.Parse(broadcast_string);
            #endregion


            string text_to_send = message;
            if (text_to_send.Length > 0)
            {
                // the socket object must have an array of bytes to send.
                // this loads the string entered by the user into an array of bytes.
                byte[] send_buffer = Encoding.ASCII.GetBytes(text_to_send);

                try
                {
                    sender.SendTo(send_buffer, sending_end_point);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(" Exception {0}", ex.Message);
                    throw;
                }
            }
        } // end of main()



    } // end of class UDPListener


    public class MyUdpClient
    {
        public MyUdpClient()
        {

        }
    }



}