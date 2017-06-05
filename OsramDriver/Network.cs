using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ThingSharp.Osram
{
    public class Network
    {
        public static byte[] SendReceive(String server, byte[] data)
        {
            try
            {
                Int32 port = 4000;
                TcpClient client = new TcpClient(server, port);
                NetworkStream stream = client.GetStream();
                stream.Write(data, 0, data.Length);
                data = new Byte[256];
                Int32 bytes = stream.Read(data, 0, data.Length);               
                stream.Close();
                client.Close();
                return data;
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            return null;
        }
    }
}
