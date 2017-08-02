using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxNet
{
	public class LightStateResponse : LifxResponseBase
	{
		public LightStateResponse(byte[] payload) : base()
		{
			Hue = BitConverter.ToUInt16(payload, 0);
			Saturation = BitConverter.ToUInt16(payload, 2);
			Brightness = BitConverter.ToUInt16(payload, 4);
			Kelvin = BitConverter.ToUInt16(payload, 6);
			IsOn = BitConverter.ToUInt16(payload, 10) > 0;
			Label = Encoding.UTF8.GetString(payload, 12, 32).Replace("\0", "");
		}
		/// <summary>
		/// Hue
		/// </summary>
		public UInt16 Hue { get; private set; }
		/// <summary>
		/// Saturation (0=desaturated, 65535 = fully saturated)
		/// </summary>
		public UInt16 Saturation { get; private set; }
		/// <summary>
		/// Brightness (0=off, 65535=full brightness)
		/// </summary>
		public UInt16 Brightness { get; private set; }
		/// <summary>
		/// Bulb color temperature
		/// </summary>
		public UInt16 Kelvin { get; private set; }
		/// <summary>
		/// Power state
		/// </summary>
		public bool IsOn { get; private set; }
		/// <summary>
		/// Light label
		/// </summary>
		public string Label { get; private set; }
	}
}
