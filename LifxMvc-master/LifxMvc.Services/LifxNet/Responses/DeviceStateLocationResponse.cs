using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxNet
{
	public class DeviceStateLocationResponse : LifxResponseBase
	{
		public Byte[] Location { get; private set; }
		public string Label { get; private set; }
		public DateTime UpdatedAt { get; private set; }
		public DeviceStateLocationResponse(byte[] payload)
		{
			if (payload != null)
			{
				using (MemoryStream ms = new MemoryStream(payload))
				{
					using (BinaryReader br = new BinaryReader(ms))
					{
						this.Location = br.ReadBytes(16);
					}
				}
				const int LABEL_LENGTH = 32;
				this.Label = Encoding.UTF8.GetString(payload, 16, LABEL_LENGTH).Replace("\0", "").Trim();
				var nanoseconds = BitConverter.ToUInt64(payload, 48);
				this.UpdatedAt = Constants.Epoch.AddMilliseconds(nanoseconds * 0.000001);
			}
		}

	}
}
