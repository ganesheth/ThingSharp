using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxNet
{
	public class DeviceStateVersionResponse : LifxResponseBase
	{
		public DeviceStateVersionResponse(byte[] payload) : base()
		{
			Vendor = BitConverter.ToUInt32(payload, 0);
			Product = BitConverter.ToUInt32(payload, 4);
			Version = BitConverter.ToUInt32(payload, 8);
		}
		/// <summary>
		/// Vendor ID
		/// </summary>
		public UInt32 Vendor { get; private set; }
		/// <summary>
		/// Product ID
		/// </summary>
		public UInt32 Product { get; private set; }
		/// <summary>
		/// Hardware version
		/// </summary>
		public UInt32 Version { get; private set; }
	}
}
