using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LifxNet;
using System.Net;

namespace LifxPacketParser.console
{
	class Program
	{
		static void Main(string[] args)
		{
			new Program().MainImpl(args);

		}

		void MainImpl(string[] args)
		{


			string str = "44:00:00:14:2b:d7:78:03:d0:73:d5:11:45:03:00:00:4c:49:46:58:56:32:02:00:00:00:00:00:00:00:00:00:18:00:00:00:6c:69:66:78:20:74:65:73:74:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00";

			var bytes = this.ParseFromWireshark(str);

			const int MESSAGE_TYPE_INDEX = 32;
			var messageType = (UInt16)bytes[MESSAGE_TYPE_INDEX];

			IPAddress broadcastIP = IPAddress.Broadcast;
			IPEndPoint endPoint = new IPEndPoint(broadcastIP, 80);


			if (Enum.IsDefined(typeof(ResponseType), messageType))
			{
				ResponseFactory.Parse(bytes, endPoint);
			}
			else if (Enum.IsDefined(typeof(PacketType), messageType))
			{
				PacketFactory.Parse(bytes, endPoint);
			}

		}


		byte[] ParseFromWireshark(string str)
		{
			var bytes = new List<byte>();
			var strings = str.Split(':');

			for(int i = 0; i < strings.Length; ++i)
			{
				var s = strings[i];
				var b = Byte.Parse(s, NumberStyles.AllowHexSpecifier);

				bytes.Add(b);

				if (0x66 == b)
					new object();

			}

			return bytes.ToArray();
		}


	}
}
