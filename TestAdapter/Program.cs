using System;
using System.IO;
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
            //First we choose what kind of protocol binding we want on top.
            //At the moment we have HTTP in stock, but expect WebSockets, CoAP etc next season.
            Uri baseUri = new Uri("http://192.168.0.121:8080/");
            ProtocolBinding httpBinding = new HTTPBinding(new string[] { "http://192.168.0.121:8080/"});

            //Next we create an Adapter. The Adapter is the stuff you will develop. It contains
            // a "logical adaption" and a driver layer.
            Adapter adapter1 = new LifxAdapter();

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
