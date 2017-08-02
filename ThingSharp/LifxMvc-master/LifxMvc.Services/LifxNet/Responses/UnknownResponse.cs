using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxNet
{
	public class UnknownResponse : LifxResponseBase
	{
		public UnknownResponse(byte[] payload) : base()
		{
		}
	}
}
