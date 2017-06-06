using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThingSharp.Osram;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            OsramDriver driver = new OsramDriver("192.168.0.139");
            List<OsramBulb> bulbs = driver.QueryBulbs();
            OsramBulb bulb = bulbs.FirstOrDefault();
            Console.WriteLine("{0}:{1}", bulb.Address, bulb.Status);

            driver.SetBulbPowerState(bulb, 0x01);
            driver.SetBulbBrightness(bulb, 0x10, 0x10);
            driver.SetBulbColor(bulb, 0x00, 0xff, 0x00, 0x10);
            driver.SetBulbColorTemperature(bulb, 0x00ff, 0x10);
            driver.SetBulbColorCycling(bulb, true, 0x10);
            driver.GetBulbStatus(ref bulb);
            Console.WriteLine("{0}:{1}", bulb.Address, bulb.Status);
        }
    }
}
