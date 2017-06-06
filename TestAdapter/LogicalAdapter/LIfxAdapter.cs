using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using ThingSharp.Bindings;
using ThingSharp.Drivers;
using ThingSharp.Server;
using ThingSharp.Types;
using ThingSharp.Utils;

namespace ThingSharp.TestAdapter
{
    class LifxAdapter : Adapter
    {
        private LifxDriver driver;

        public LifxAdapter(IPAddress localEndpoint)
        {
            driver = new LifxDriver(localEndpoint);
        }
        public override void Initialize(Uri baseUri)
        {
            Console.WriteLine("Initializing adapter..");
            List<object> bulbs = driver.DiscoverBulbs(1);
            int i = 1;
            foreach(object b in bulbs)
            {
                GMS_OM_LED_LAMP lifxLamp = new GMS_OM_LED_LAMP(new Uri(baseUri, String.Format("lifx_{0}", i++)));
                lifxLamp.Id = String.Format("lifx_{0}", i++);
                lifxLamp.SubsystemContext = b;
                lifxLamp.RemoveProperty("Brightness");
                RaiseOnThingAdded(lifxLamp);
            }

            Console.WriteLine("Found {0} bulbs", bulbs.Count);

            //Instances can be created from imported types (.net classes)
            GMS_OM_LED_LAMP lightFromStaticType = new GMS_OM_LED_LAMP(new Uri(baseUri, "L1"));
            lightFromStaticType.RemoveProperty("Brightness");
            RaiseOnThingAdded(lightFromStaticType);

            //Now, the variant of creating instances directly from RDF
            //Thing lightFromRdf = ThingModelParser.FromRDF(@"D:\Data\GMS\RESTDriverProject\Prototypes\GMS_OM.owl", "#GMS_OM_OSRAM_1", new Uri(baseUri, "L2"));
            //var v = lightFromRdf.Properties["Brightness"]; 
                   
                       
            //Ah yes.. dont forget to let everyone know that you have a new Thing.
            //RaiseOnThingAdded(lightFromRdf);
        }

        public override object Read(Resource obj)
        {
            if (obj is Thing)
                return obj;

            PropertyBase property = (PropertyBase)obj;
    
            if (property.Name == "Switch")
            {
                bool status = driver.GetBulbStatus(property.Parent.SubsystemContext);
                return status;
            }

            if (property.Name == "Color")
            {
                string color = driver.GetBulbColor(property.Parent.SubsystemContext);
                return color;
            }

            return null;
        }

        public override object Write(Resource obj, object value)
        {
            if (obj is Thing)
                return "Thing Description";

            PropertyBase property = (PropertyBase)obj;

            if (property.Name == "Switch")
            {
                bool status = driver.SetBulbStatus(property.Parent.SubsystemContext,(bool)value);
                return status;
            }

            if (property.Name == "Color")
            {
                bool status = (bool)driver.SetBulbColor(property.Parent.SubsystemContext, (string)value);
                return status;
            }

            return null;
        }
    }
}
