using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxNet
{
	public class DeviceStateWifiInfoResponse : LifxResponseBase
	{
		public Single Signal { get; private set; }
		public UInt32 TxCount { get; private set; }
		public UInt32 RxCount { get; private set; }
		public DeviceStateWifiInfoResponse(byte[] payload) : base()
		{
			Signal = BitConverter.ToSingle(payload, 0);
			TxCount = BitConverter.ToUInt32(payload, 4);
			RxCount = BitConverter.ToUInt32(payload, 8);
		}
	}//class
}
