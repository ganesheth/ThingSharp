using Colorspace;
using LifxMvc.Domain;
using System;
using System.Collections.Generic;
using Windows = System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace LifxMvc.Domain
{
	public static class ColorExtensions
	{
		static public HSBK ToHSBK(this Windows.Color wrgb, bool isKelvin = false)
		{
			HSBK result = null;
			if (isKelvin)
			{
				var kc = wrgb.ToKelvinColor();
#warning FIXME: Need Brightness....
				result = new HSBK(kc.Temperature, UInt16.MaxValue);
			}
			else
			{
                ColorRGB rgb = new ColorRGB(wrgb.ToArgb());
                var hsl = new ColorHSL(rgb);
				result = new HSBK(hsl.H, hsl.S, hsl.L);
			}
			return result;
		}

		static public ColorHSL ToColorHSL(this IHSBK hsbk)
		{
			double h, s, b;
			hsbk.GetHSB(out h, out s, out b);
			var result = new ColorHSL(h, s, b);
			return result;
		}

		static public KelvinColor ToKelvinColor(this Windows.Color color)
		{
			var result = KelvinColor.Create(color);
			return result;
		}

        static public Windows.Color ToColor(this IHSBK hsbk, bool isKelvin = false)
		{
			var result = Windows.Color.AliceBlue;
            if (!isKelvin)
			{
				var hsl = hsbk.ToColorHSL();
				var rgb = new ColorRGB(hsl);
				var rgb32 = new ColorRGB32Bit(rgb);

				result = Windows.Color.FromArgb(rgb32.ToInt());
			}
			else
			{
				result = ColorFromTemperature(hsbk.Kelvin);
			}
			return result;
		}

		//Given a temperature (in Kelvin), estimate an RGB equivalent
		// Taken from: http://www.tannerhelland.com/4435/convert-temperature-rgb-algorithm-code/
		public static Windows.Color ColorFromTemperature(int tmpKelvin)
		{
			double tmpCalc;
			double r, g, b;

			//Temperature must fall between 1000 and 40000 degrees
			if (tmpKelvin < 1000)
				tmpKelvin = 1000;

			if (tmpKelvin > 40000)
				tmpKelvin = 40000;

			//All calculations require tmpKelvin \ 100, so only do the conversion once
			tmpKelvin /= 100;

			//Calculate each color in turn

			#region First: red

			if (tmpKelvin <= 66)
			{
				r = 255;
			}
			else
			{
				//Note: the R-squared value for this approximation is .988
				tmpCalc = tmpKelvin - 60;
				tmpCalc = 329.698727446 * Math.Pow(tmpCalc, -0.1332047592);

				r = tmpCalc;
				r = Clamp(r, 0, 255);
			}
			#endregion

			#region Second: green

			if (tmpKelvin <= 66)
			{
				//Note: the R-squared value for this approximation is .996
				tmpCalc = tmpKelvin;

				tmpCalc = 99.4708025861 * Math.Log(tmpCalc) - 161.1195681661;

				g = tmpCalc;
				g = Clamp(g, 0, 255);
			}
			else
			{
				//Note: the R-squared value for this approximation is .987
				tmpCalc = tmpKelvin - 60;
				tmpCalc = 288.1221695283 * Math.Pow(tmpCalc, -0.0755148492);

				g = tmpCalc;
				g = Clamp(g, 0, 255);
			}

			#endregion

			#region Third: blue

			if (tmpKelvin >= 66)
			{
				b = 255;
			}
			else if (tmpKelvin <= 19)
			{
				b = 0;
			}
			else
			{
				//Note: the R-squared value for this approximation is .998
				tmpCalc = tmpKelvin - 10;
				tmpCalc = 138.5177312231 * Math.Log(tmpCalc) - 305.0447927307;

				b = tmpCalc;
				b = Clamp(b, 0, 255);
			}

			#endregion

			var rr = (int)Math.Round(r);
			var gg = (int)Math.Round(g);
			var bb = (int)Math.Round(b);

			var result = Windows.Color.FromArgb(rr, gg, bb);
			return result;
		}

		static double Clamp(double x, double min, double max)
		{
			var result = x;
			if (x < min)
			{
				result = min;
			}
			if (x > max)
			{
				result = max;
			}

			return result;
		}

	}//class
}//ns
