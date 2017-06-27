using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThingSharp.Osram;
using ThingSharp.Types;
using RestAdapter;

namespace ThingSharp.TestAdapter
{
    public class OsramAdapter : Adapter
    {
        private OsramDriver driver = new OsramDriver("192.168.0.139");

        public override void Initialize(Uri baseUri)
        {
            Console.WriteLine("Initializing adapter..");
            List<OsramBulb> bulbs = driver.QueryBulbs();
            int i = 1;
            foreach (object b in bulbs)
            {
                LIFX_WIFI_LIGHT osramLamp = new LIFX_WIFI_LIGHT(new Uri(baseUri, String.Format("osram_{0}", i++)));
                osramLamp.SubsystemContext = b;
                RaiseOnThingAdded(osramLamp);
            }

            Console.WriteLine("Found {0} bulbs", bulbs.Count);
        }

        public override object Read(Resource obj)
        {
            if (obj is Thing)
                return obj;

            PropertyBase property = (PropertyBase)obj;
            OsramBulb bulb = property.Parent.SubsystemContext as OsramBulb;

            if (property.Name == "Switch")
            {
                driver.GetBulbStatus(ref bulb);
                return bulb.Status;
            }

            if (property.Name == "Brightness")
            {
                driver.GetBulbStatus(ref bulb);
                return bulb.Brightness;
            }

            if (property.Name == "Color")
            {
                driver.GetBulbStatus(ref bulb);
                return bulb.LightColorAsString;
            }
            return null;
        }

        public override object Write(Resource obj, object value)
        {
            if (obj is Thing)
                return "Thing Description";

            PropertyBase property = (PropertyBase)obj;
            OsramBulb bulb = property.Parent.SubsystemContext as OsramBulb;

            if (property.Name == "Switch")
            {
                bool v = (bool)value;
                bool status = driver.SetBulbPowerState(bulb, v ? (byte)0x01 : (byte)0x00);
                return status;
            }

            if (property.Name == "Brightness")
            {
                byte v = System.Convert.ToByte(value); ;
                bool status = (bool)driver.SetBulbBrightness(bulb, v, 0);
                return status;
            }

            if (property.Name == "Color")
            {
                bool status = (bool)driver.SetBulbColor(bulb, (string)value, 0);
                return status;
            }

            return null;
        }
    }
}
