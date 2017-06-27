﻿using LifxMvc.Domain;
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
        private DiscoveryService mDiscoveryService;
        private BulbService mBulbService;

        public LifxDriver(IPAddress localEndpoint)
        {
            DiscoveryUdpHelper.LocalEndpointIpAddress = localEndpoint;
            mDiscoveryService = new DiscoveryService();
            mBulbService = new BulbService();
        }

        //--------------------------------------------------------------------
        // Discovery
        //--------------------------------------------------------------------

        public void DiscoverBulbs()
        {
            // Tell the Lifx Service to start discovering bulbs
            mDiscoveryService.DiscoverAsync();
        }

        public List<Object> GetNewBulbs()
        {
            // Get a list of all the newly discovered bulbs
            List<IBulb> bulbs = mDiscoveryService.GetDiscoveredBulbs();
            List<object> bulbIds = new List<object>();
            foreach (IBulb bulb in bulbs)
            {
                bulbIds.Add(bulb);
            }
            
            return bulbIds;
        }

        //--------------------------------------------------------------------
        // Bulb Label - Get
        //--------------------------------------------------------------------

        public string GetBulbLabel(Object bulb)
        {
            IBulb b = (IBulb)bulb;
            mBulbService.GetLabel(b);
            return b.Label;
        }

        public string GetBulbObjectLabel(Object bulb)
        {
            IBulb b = (IBulb)bulb;

            if (String.IsNullOrEmpty(b.Label))
            {
                System.Diagnostics.Debug.WriteLine("----- Bulb Label not set ----");
            }

            return b.Label;
        }

        public string GetBulbEndPoint(Object bulb)
        {
            IBulb b = (IBulb)bulb;
            return b.IPEndPoint.ToString();
        }

        //--------------------------------------------------------------------
        // Bulb Power - Get/Set
        //--------------------------------------------------------------------

        public uint? GetBulbPower(Object bulb)
        {
            IBulb b = (IBulb)bulb;
            return mBulbService.LightGetPower(b);
        }
        public bool SetBulbPower(Object bulb, object state)
        {
            IBulb b = (IBulb)bulb;
            return mBulbService.LightSetPower(b, (bool)state);
        }

        //--------------------------------------------------------------------
        // Bulb Color - Get/Set
        //--------------------------------------------------------------------

        public string GetBulbColor(Object bulb)
        {
            IBulb b = (IBulb)bulb;
            return mBulbService.LightGetColor(b);
        }
        public object SetBulbColor(Object bulb, object value)
        {
            IBulb b = (IBulb)bulb;
            System.Drawing.Color color = ParseColor((string)value);
            return mBulbService.LightSetColor(b, color);
        }

        System.Drawing.Color ParseColor(string color)
        {
            // Remove '#' from front if present
            int argb = int.Parse(color.Replace("#", ""), System.Globalization.NumberStyles.HexNumber);

            // Add Alpha to RGB value if missing
            if(color.Length == 6)
            {
                color = "FF" + color;
            }

            return System.Drawing.Color.FromArgb(argb);            
        }

        //--------------------------------------------------------------------
        // Bulb Brightness - Get/Set
        //--------------------------------------------------------------------

        public ushort? GetBulbBrightness(Object bulb)
        {
            IBulb b = (IBulb)bulb;
            ushort? brightness = mBulbService.LightGetBrightness(b);

            if (brightness != null)
            {
                // Convert brightness to Percentage value
                brightness = (ushort)Math.Round(((float)brightness / 65535.0) * 100.0, 0);
            }

            return brightness;
        }
        public object SetBulbBrightness(Object bulb, object value)
        {
            ushort brightness = Convert.ToUInt16(value);

            // If the conversion to an int failed, then return false
            if (brightness < 0)
                return false;

            IBulb b = (IBulb)bulb;

            // convert from % to value in range of 0 to 65535
            brightness = brightness > (ushort)100 ? (ushort)100 : brightness;
            brightness = brightness < (ushort)0 ? (ushort)0 : brightness;
            brightness = (ushort)Math.Round(((float)brightness / 100.0) * 65535.0, 0);

            return mBulbService.LightSetBrightness(b, brightness);
        }

        //--------------------------------------------------------------------
        // Bulb Saturation - Get/Set
        //--------------------------------------------------------------------

        public ushort? GetBulbSaturation(Object bulb)
        {
            IBulb b = (IBulb)bulb;
            ushort? saturation = mBulbService.LightGetSaturation(b);

            if (saturation != null)
            {
                // Convert saturation to Percentage value
                saturation = (ushort)Math.Round(((float)saturation / 65535.0) * 100.0, 0);
            }

            return saturation;
        }
        public object SetBulbSaturation(Object bulb, object value)
        {
            ushort saturation = Convert.ToUInt16(value);

            // If the conversion to an int failed, then return false
            if (saturation < 0)
                return false;

            IBulb b = (IBulb)bulb;

            // convert from % to value in range of 0 to 65535
            saturation = saturation > (ushort)100 ? (ushort)100 : saturation;
            saturation = saturation < (ushort)0 ? (ushort)0 : saturation;
            saturation = (ushort)Math.Round(((float)saturation / 100.0) * 65535.0, 0);

            return mBulbService.LightSetSaturation(b, saturation);
        }

        //--------------------------------------------------------------------
        // Bulb Temperature (Kelvin) - Get/Set
        //--------------------------------------------------------------------

        public ushort? GetBulbKelvin(Object bulb)
        {
            IBulb b = (IBulb)bulb;
            return mBulbService.LightGetKelvin(b);
        }
        public object SetBulbKelvin(Object bulb, object value)
        {
            ushort kelvin = Convert.ToUInt16(value);

            // If the conversion to an int failed, then return false
            if (kelvin < 0)
                return false;

            IBulb b = (IBulb)bulb;

            // convert from % to value in range of 0 to 65535
            kelvin = kelvin > (ushort)6500 ? (ushort)6500 : kelvin;
            kelvin = kelvin < (ushort)2700 ? (ushort)2700 : kelvin;

            return mBulbService.LightSetKelvin(b, kelvin);
        }
    }

}
