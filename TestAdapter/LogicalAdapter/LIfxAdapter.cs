using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using ThingSharp.Bindings;
using ThingSharp.Drivers;
using ThingSharp.Server;
using ThingSharp.Types;
using ThingSharp.Utils;
using RestAdapter;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ThingSharp.TestAdapter
{
    class LifxAdapter : Adapter
    {
        private LifxDriver driver = null;

        public LifxAdapter(IPAddress localEndpoint)
        {
            driver = new LifxDriver(localEndpoint);
        }

        public override void Initialize(Uri baseUri)
        {
            Console.WriteLine("Initializing adapter..");

            driver.DiscoverBulbs();
            StartCheckingForNewBulbs(baseUri);

            // TEST CODE //
            //Instances can be created from imported types (.net classes)
            //LIFX_WIFI_LIGHT lightFromStaticType = new LIFX_WIFI_LIGHT(new Uri(baseUri, "L1"));
            //lightFromStaticType.RemoveProperty("Brightness");
            //RaiseOnThingAdded(lightFromStaticType);

            //Now, the variant of creating instances directly from RDF
            //Thing lightFromRdf = ThingModelParser.FromRDF(@"D:\Data\GMS\RESTDriverProject\Prototypes\GMS_OM.owl", "#GMS_OM_OSRAM_1", new Uri(baseUri, "L2"));
            //var v = lightFromRdf.Properties["Brightness"]; 
                   
                       
            //Ah yes.. dont forget to let everyone know that you have a new Thing.
            //RaiseOnThingAdded(lightFromRdf);
        }

        void StartCheckingForNewBulbs(Uri baseUri)
        {
            var listenProc = new Thread(CheckForNewBulbs);
            listenProc.Start(baseUri);
        }

        void CheckForNewBulbs(object ob)
        {
            try
            {
                Uri baseUri = ob as Uri;
                int i = 1;

                while (true)
                {
                    List<object> bulbs = driver.GetNewBulbs();

                    foreach (object b in bulbs)
                    {
                        string id_string;
                        string label = driver.GetBulbObjectLabel(b);
                        if (String.IsNullOrEmpty(label))
                        {
                            id_string = String.Format("lifx_{0}", i++);
                        }
                        else
                        {
                            id_string = String.Format("lifx_{0}", label.Replace(' ', '_'));
                        }                    

                        LIFX_WIFI_LIGHT lifxLamp = new LIFX_WIFI_LIGHT(new Uri(baseUri, id_string));

                        lifxLamp.Id = id_string;
                        lifxLamp.SubsystemContext = b;

                        RaiseOnThingAdded(lifxLamp);

                        Console.WriteLine("Label: " + id_string + ":" + driver.GetBulbEndPoint(b));
                    }

                    if (bulbs.Count > 0)
                    {
                        Console.WriteLine("Found {0} new bulb(s)", bulbs.Count);
                    }

                    // Wait some amount of time before checking for new bulbs
                    System.Threading.Thread.Sleep(10 * 1000);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
        }

        static int count = 1;

        public override object Read(Resource obj)
        {
            Stopwatch sw = Stopwatch.StartNew();
                       

            if (obj is Thing)
                return obj;

            object value = null;

            PropertyBase property = (PropertyBase)obj;

            switch (property.Name)
            {
                case "Power":
                    value = driver.GetBulbPower(property.Parent.SubsystemContext); 
                    break;

                case "Color":
                    value = driver.GetBulbColor(property.Parent.SubsystemContext); 
                    break;

                case "Brightness":
                    value = driver.GetBulbBrightness(property.Parent.SubsystemContext);
                    break;

                case "Saturation":
                    value = driver.GetBulbSaturation(property.Parent.SubsystemContext);
                    break;

                case "Kelvin":
                    value = driver.GetBulbKelvin(property.Parent.SubsystemContext); 
                    break;

                case "Label":
                    value = driver.GetBulbObjectLabel(property.Parent.SubsystemContext);
                    break;

                default:
                    break;
            }
                
            sw.Stop();
            Console.WriteLine("GET:{0}\t -- TimeElapsed: {1}   ({2})  {3}", property.Name, sw.Elapsed, count++, driver.GetBulbObjectLabel(property.Parent.SubsystemContext));

            return value;
        }

        public override object Write(Resource obj, object value)
        {
            Stopwatch sw = Stopwatch.StartNew();

            if (obj is Thing)
                return "Thing Description";

            object returnStatus = null;

            PropertyBase property = (PropertyBase)obj;

            switch (property.Name)
            {
                case "Power": 
                    returnStatus = driver.SetBulbPower(property.Parent.SubsystemContext, value); 
                    break;

                case "Color": 
                    returnStatus = driver.SetBulbColor(property.Parent.SubsystemContext, value); 
                    break;

                case "Brightness": 
                    returnStatus = driver.SetBulbBrightness(property.Parent.SubsystemContext, value); 
                    break;

                case "Saturation": 
                    returnStatus = driver.SetBulbSaturation(property.Parent.SubsystemContext, value); 
                    break;

                case "Kelvin": 
                    returnStatus = driver.SetBulbKelvin(property.Parent.SubsystemContext, value); 
                    break;

                default: 
                    break;
            }

            sw.Stop();
            Console.WriteLine("PUT:{0}\t -- TimeElapsed: {1}", property.Name, sw.Elapsed);

            return returnStatus;
        }
    }
}
