using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace ThingSharp.Utils
{
    public class IpAddressHelper
    {
        //public static String IpAddress { get; set; }      
  
        public string GetLocalIPv4(NetworkInterfaceType type)
        {
            string foundIP = "";

            foreach(NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if(item.NetworkInterfaceType == type && item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach(UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if(ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            foundIP = ip.Address.ToString();
                        }
                    }
                }
            }
            return foundIP;            
        }
    }   
}
