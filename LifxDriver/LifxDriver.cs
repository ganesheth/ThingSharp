using LifxMvc.Domain;
using LifxMvc.Services;
using LifxMvc.Services.UdpHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ThingSharp.Drivers
{
    public class LifxDriver
    {
        private List<IBulb> mLastDiscoveredBulbs = new List<IBulb>();
        private DiscoveryService mDiscoveryService;
        private BulbService mBulbService;

        public LifxDriver(IPAddress localEndpoint)
        {
            DiscoveryUdpHelper.LocalEndpointIpAddress = localEndpoint;
            mDiscoveryService = new DiscoveryService();
            mBulbService = new BulbService();
        }
        public List<Object> DiscoverBulbs(int expectedNumber)
        {            
            List<IBulb> bulbs = mDiscoveryService.DiscoverAsync(1);
            List<object> bulbIds = new List<object>();
            foreach(IBulb bulb in bulbs)
            {
                bulbIds.Add(bulb);
            }
            mLastDiscoveredBulbs = bulbs;
            return bulbIds;
        }

        public bool GetBulbStatus(Object bulb)
        {
            IBulb b = (IBulb)bulb;
            bool status = mBulbService.LightGetPower(b);
            return status;
        }
        public bool SetBulbStatus(Object bulb, bool state)
        {
            IBulb b = (IBulb)bulb;
            mBulbService.LightSetPower(b, state);
            return true;
        }

        public string GetBulbColor(Object bulb)
        {
            IBulb b = (IBulb)bulb;
            mBulbService.LightGet(b);
            return String.Format("{0:X2}{1:X2}{2:X2}{3:X2}", b.Color.A, b.Color.R, b.Color.G, b.Color.B);
        }

        public object SetBulbColor(Object bulb, string colorName)
        {
            IBulb b = (IBulb)bulb;
            System.Drawing.Color color = ParseColor(colorName);
            mBulbService.LightSetColor(b, color);
            return true;
        }
        System.Drawing.Color ParseColor(string color)
        {
            int argb = int.Parse(color.Replace("#", ""), System.Globalization.NumberStyles.HexNumber);
            return System.Drawing.Color.FromArgb(argb);
            /*
            //Parse: rgb(255, 0, 137)
            const string REGEX = @"\D+";
            var values = Regex.Split(color, REGEX).Where(x => !string.IsNullOrEmpty(x)).ToArray();

            const int EXPECTED_LENGTH = 3;
            if (EXPECTED_LENGTH != values.Length)
                throw new Exception("Error parsing color.");

            var result = System.Drawing.Color.FromArgb(Convert.ToInt32(values[0]),
                Convert.ToInt32(values[1]),
                Convert.ToInt32(values[2]));

            return result;
            */
        }
    }

}
