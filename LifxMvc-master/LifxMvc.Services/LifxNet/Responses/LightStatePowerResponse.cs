using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxNet
{
	public class LightStatePowerResponse : LifxResponseBase
	{
		public bool IsOn { get; private set; }
		public LightStatePowerResponse(byte[] payload) : base()
		{
			IsOn = BitConverter.ToUInt16(payload, 0) > 0;
		}
	}
}
