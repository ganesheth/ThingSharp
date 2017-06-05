using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LifxMvc.Domain.ColorExtensions;

namespace LifxMvc.Domain
{
	public class HSBK : IHSBK
	{
		public UInt16 Hue { get; private set; }
		public UInt16 Saturation { get; private set; }
		public UInt16 Brightness { get; private set; }
		public UInt16 Kelvin { get; private set; }
		public bool IsColor { get; set; }

		private HSBK(UInt16 h, UInt16 s, UInt16 b, UInt16 k)
		{
			this.Hue = h;
			this.Saturation = s;
			this.Brightness = b;
			this.Kelvin = k;
		}
		public HSBK(UInt16 h, UInt16 s, UInt16 b)
		{
			this.Hue = h;
			this.Saturation = s;
			this.Brightness = b;
			this.Kelvin = 0;
			this.IsColor = true;
		}

		public HSBK(UInt16 kelvin, UInt16 brightness)
		{
			this.Kelvin = kelvin;
			this.Brightness = brightness;
			this.IsColor = false;
		}
		public HSBK(double h, double s, double b)
		{
			this.Hue = this.ToUInt16(h);
			this.Saturation = this.ToUInt16(s);

			this.Brightness = this.ToUInt16(b);
			this.Kelvin = 0;
			this.IsColor = true;
		}
		public static HSBK Create(UInt16 h, UInt16 s, UInt16 b, UInt16 k)
		{
			return new HSBK(h, s, b, k);
		}

		public static HSBK Create(KelvinColor kc)
		{
			return new HSBK(kc.Temperature, UInt16.MaxValue);
		}

		public void GetHSB(out double h, out double s, out double b)
		{
			h = (double)this.Hue / (double)UInt16.MaxValue;
			s = (double)this.Saturation / (double)UInt16.MaxValue;
			b = (double)this.Brightness / (double)UInt16.MaxValue;
		}

		UInt16 ToUInt16(double d)
		{
			UInt16 result = 0;
			if (d > 0)
			{
				result = (UInt16)Math.Round(UInt16.MaxValue * d); 
			}

			return result;
		}

		public void RotateHue()
		{
			if (0 != this.Saturation)
			{
				this.Hue += (UInt16)64;// Int16.MaxValue;
									   //Console.Beep();
			}
		}

		public void RotateHue(UInt16 degrees)
		{
			UInt16 degreeWidth = UInt16.MaxValue / 360;
			UInt16 rotation = (UInt16)(degreeWidth * degrees);

			if (0 != this.Saturation)
			{
				this.Hue += rotation;
									   //Console.Beep();
			}
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append(base.ToString());
			var s = string.Format(": Hue={0}, Saturation={1}, Brightness={2}",
				this.Hue,
				this.Saturation,
				this.Brightness);
			sb.AppendLine(s);
			return sb.ToString();
		}


	}
}
