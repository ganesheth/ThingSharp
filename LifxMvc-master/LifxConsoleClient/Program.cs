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
            DiscoveryUdpHelper.LocalEndpointIpAddress = new IPAddress(new byte[] { 192, 168, 0, 114 });
            DiscoveryService disco = new DiscoveryService();
            List<IBulb> bulbs = disco.DiscoverAsync(1);
            BulbService bulbService = new BulbService();            
            IBulb bulb = bulbs[0];
            ushort b = bulb.Brightness;
            bulbService.LightSetPower(bulb, true);
            bulbService.LightSetColor(bulb, System.Drawing.Color.FromArgb(0, 0, 0, 128));
            bulbService.LightGet(bulb);
            b = bulb.Brightness;
            bulbService.LightSetColor(bulb, System.Drawing.Color.FromArgb(0, 0, 128, 0));
            bulbService.LightGet(bulb);
            b = bulb.Brightness;
            bulbService.LightSetPower(bulb, false);
        }
    }
}
