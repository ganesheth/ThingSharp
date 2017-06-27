using LifxMvc.Domain;
using LifxMvc.Services;
using LifxMvc.Services.UdpHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LifxConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            DiscoveryUdpHelper.LocalEndpointIpAddress = new IPAddress(new byte[] { 192, 168, 1, 111 });
            DiscoveryService disco = new DiscoveryService();
            //List<IBulb> bulbs = disco.DiscoverAsync();
            //BulbService bulbService = new BulbService();            
            //IBulb bulb = bulbs[0];
            ////ushort b = bulb.Brightness;
            ////bulbService.LightSetPower(bulb, true);
            ////System.Threading.Thread.Sleep(500);
            ////bulbService.LightSetColor(bulb, System.Drawing.Color.FromArgb(255, 0, 0, 128));
            ////System.Threading.Thread.Sleep(500);
            ////bulbService.LightGet(bulb);
            ////System.Threading.Thread.Sleep(500);
            ////b = bulb.Brightness;
            ////bulbService.LightSetColor(bulb, System.Drawing.Color.FromArgb(0, 0, 128, 0));
            ////System.Threading.Thread.Sleep(500);
            ////bulbService.LightGet(bulb);
            ////System.Threading.Thread.Sleep(500);
            ////b = bulb.Brightness;
            ////bulbService.LightSetPower(bulb, false);
        }
    }
}
