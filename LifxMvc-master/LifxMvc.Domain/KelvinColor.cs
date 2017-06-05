using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace LifxMvc.Domain
{
	public class KelvinColor 
	{
		public enum KelvinColorsEnum
		{// Values corrospond to Color.ToArgb() values
			Kelvin2500 = -24762,
			Kelvin2750 = -22697,
			Kelvin3000 = -20114,
			Kelvin3200 = -18309,
			Kelvin3500 = -15987,
			Kelvin4000 = -12634,
			Kelvin4500 = -9541,
			Kelvin5000 = -6962,
			Kelvin5500 = -4642,
			Kelvin6000 = -2323,
			Kelvin6500 = -262,
			Kelvin7000 = -789761,
			Kelvin7500 = -1643521,
			Kelvin8000 = -2234625,
			Kelvin8500 = -2628865,
			Kelvin9000 = -2957313,
		};

		static public Color Kelvin2500 = Color.FromArgb((int)KelvinColorsEnum.Kelvin2500);
		static public Color Kelvin2750 = Color.FromArgb((int)KelvinColorsEnum.Kelvin2750);
		static public Color Kelvin3000 = Color.FromArgb((int)KelvinColorsEnum.Kelvin3000);
		static public Color Kelvin3200 = Color.FromArgb((int)KelvinColorsEnum.Kelvin3200);
		static public Color Kelvin3500 = Color.FromArgb((int)KelvinColorsEnum.Kelvin3500);
		static public Color Kelvin4000 = Color.FromArgb((int)KelvinColorsEnum.Kelvin4000);
		static public Color Kelvin4500 = Color.FromArgb((int)KelvinColorsEnum.Kelvin4500);
		static public Color Kelvin5000 = Color.FromArgb((int)KelvinColorsEnum.Kelvin5000);
		static public Color Kelvin5500 = Color.FromArgb((int)KelvinColorsEnum.Kelvin5500);
		static public Color Kelvin6000 = Color.FromArgb((int)KelvinColorsEnum.Kelvin6000);
		static public Color Kelvin6500 = Color.FromArgb((int)KelvinColorsEnum.Kelvin6500);
		static public Color Kelvin7000 = Color.FromArgb((int)KelvinColorsEnum.Kelvin7000);
		static public Color Kelvin7500 = Color.FromArgb((int)KelvinColorsEnum.Kelvin7500);
		static public Color Kelvin8000 = Color.FromArgb((int)KelvinColorsEnum.Kelvin8000);
		static public Color Kelvin8500 = Color.FromArgb((int)KelvinColorsEnum.Kelvin8500);
		static public Color Kelvin9000 = Color.FromArgb((int)KelvinColorsEnum.Kelvin9000);

		Color _color;
		public UInt16 Temperature { get; private set; }
		public KelvinColor(KelvinColorsEnum kc)
		{
			_color = Color.FromArgb((int)kc);
			this.Temperature = this.GetTemperature(kc);

		}

		static public KelvinColor Create(Color c)
		{
			var kce = (KelvinColorsEnum)c.ToArgb();
			var result = new KelvinColor(kce);
			return result;
		}

		UInt16 GetTemperature(KelvinColorsEnum kc)
		{
			UInt16 kelvinTemp = UInt16.MaxValue;
			switch (kc)
			{
				case KelvinColorsEnum.Kelvin2500:
					kelvinTemp = 2500;
					break;
				case KelvinColorsEnum.Kelvin2750:
					kelvinTemp = 2750;
					break;
				case KelvinColorsEnum.Kelvin3000:
					kelvinTemp = 3000;
					break;
				case KelvinColorsEnum.Kelvin3200:
					kelvinTemp = 3200;
					break;
				case KelvinColorsEnum.Kelvin3500:
					kelvinTemp = 3500;
					break;
				case KelvinColorsEnum.Kelvin4000:
					kelvinTemp = 4000;
					break;
				case KelvinColorsEnum.Kelvin4500:
					kelvinTemp = 4500;
					break;
				case KelvinColorsEnum.Kelvin5000:
					kelvinTemp = 5000;
					break;
				case KelvinColorsEnum.Kelvin5500:
					kelvinTemp = 5500;
					break;
				case KelvinColorsEnum.Kelvin6000:
					kelvinTemp = 6000;
					break;
				case KelvinColorsEnum.Kelvin6500:
					kelvinTemp = 6500;
					break;
				case KelvinColorsEnum.Kelvin7000:
					kelvinTemp = 7000;
					break;
				case KelvinColorsEnum.Kelvin7500:
					kelvinTemp = 7500;
					break;
				case KelvinColorsEnum.Kelvin8000:
					kelvinTemp = 8000;
					break;
				case KelvinColorsEnum.Kelvin8500:
					kelvinTemp = 8500;
					break;
				case KelvinColorsEnum.Kelvin9000:
					kelvinTemp = 9000;
					break;
				default:
					break;
			}
			return kelvinTemp;
		}

		static public List<string> GetPalette()
		{
			const string RGB_FORMAT = "\"rgb({0},{1},{2})\",";
			var result = new List<string>();
			var colors = GetColors();
			colors.ForEach(x => result.Add(string.Format(RGB_FORMAT, x.R, x.G, x.B)));
			return result;
		}

		static List<Color> GetColors()
		{
			var result = new List<Color>()
				{
					Kelvin2500,
					Kelvin2750,
					Kelvin3000,
					Kelvin3200,
					Kelvin3500,
					Kelvin4000,
					Kelvin4500,
					Kelvin5000,
					Kelvin5500,
					Kelvin6000,
					Kelvin6500,
					Kelvin7000,
					Kelvin7500,
					Kelvin8000,
					Kelvin8500,
					Kelvin9000
				};
			return result;
		}

	}//class
}//ns
