using System;
using System.IO;
using System.Net;
using ThingSharp.Bindings;
using ThingSharp.Server;
using ThingSharp.Types;
using ThingSharp.Utils;

namespace ThingSharp.TestAdapter
{
    class Program
    {
        static void Main(string[] args)
        {
            IPAddress localEndpoint;
            int httpPort;

            if (args.Length > 0)
            {
                localEndpoint = IPAddress.Parse(args[0]);
                httpPort = int.Parse(args[1]);
                Console.WriteLine("EndPoint (manual): " + localEndpoint + ":" + httpPort.ToString());
            }
            else
            {
                // Find the IP Address of the Wireless Connection
                IpAddressHelper IpHelper = new IpAddressHelper();
                string ipString = IpHelper.GetLocalIPv4(System.Net.NetworkInformation.NetworkInterfaceType.Wireless80211);
                if (!string.IsNullOrEmpty(ipString))
                {
                    localEndpoint = IPAddress.Parse(ipString);
                    httpPort = 8080;
                    Console.WriteLine("EndPoint (automatic): " + localEndpoint + ":8080");
                }
                else
                {
                    Console.WriteLine("Error: Could not automatically retreive the IP address for the Wireless adapter. \n\nPlease make sure the wireless adapter is turned on and connected to the network.");
                    Console.Read();
                    return;
                }
            }
            String httpEnpoint = String.Format("http://{0}:{1}/", localEndpoint.ToString(), httpPort);

            //First we choose what kind of protocol binding we want on top.
            //At the moment we have HTTP in stock, but expect WebSockets, CoAP etc next season.
            Uri baseUri = new Uri(httpEnpoint);
            ProtocolBinding httpBinding = new HTTPBinding(new string[] { httpEnpoint });

            //Next we create an Adapter. The Adapter is the stuff you will develop. It contains
            // a "logical adaption" and a driver layer.
            Adapter adapter1 = new LifxAdapter(localEndpoint);

            //Now, we glue the protocol binding and the adapter using the ThingServer.
            ThingServer server = new ThingServer(httpBinding, adapter1);

            //Things are glued. Lets kick start the adapter. This should prompt it to setup its driver,
            //discover datapoints in the sub-system below and then create intances of Things (which will be stored 
            //in the resource container of the ThingServer).
            adapter1.Initialize(baseUri);

            //Finally, now that the Driver, Adapter, and the ThingServer are ready to serve out Things, we
            //will ask the ThingServer to open its north-side doors! (i.e. enable the protocol binding endpoint to listen).
            server.Start();

            //Thats it. Press any key to end the show.
            Console.Read();
            server.Stop();
        }
    }
}
