using Microsoft.VisualStudio.TestTools.UnitTesting;
using LifxMvc.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Eric.Morrison;
using System.Diagnostics;
using Colorspace;
using LifxMvc.Domain;

namespace LifxMvc.Domain.Tests
{
	[TestClass()]
	public class ColorExtensionsTests
	{
		[TestMethod()]
		public void ColorFromTemperatureTest()
		{
			var list = new List<int>() { 2500, 2750, 3000, 3200, 3500, 4000, 4500, 5000, 5500, 6000, 6500, 7000, 7500, 8000, 8500, 9000 };
			foreach (var k in list)
			{
				var color = ColorExtensions.ColorFromTemperature(k);
				const string RGB_FORMAT = "\"rgb({0},{1},{2})\",";
				var s = string.Format(RGB_FORMAT, color.R, color.G, color.B);
				Debug.WriteLine(s);

				//const string RGB_FORMAT = "Kelvin{0} = {1},";
				//var s = string.Format(RGB_FORMAT, k, color.ToArgb());
				//Debug.WriteLine(s);
			}
		}
	}
}

namespace LifxMvc.Services.Tests
{
	[TestClass()]
	public class ColorExtensionsTests
	{
		const int MAX_COUNT = 100 * 10000;

		[TestMethod()]
		public void ToHSBKTest()
		{
			for (int i = 0; i < MAX_COUNT; ++i)
			{
				//var a = Color.FromArgb(RandomValue.Next(0, 256), RandomValue.Next(0, 256), RandomValue.Next(0, 256));
				//var rgb = new ColorRGB(a.ToArgb());
				//var hsl = new ColorHSL(rgb);
				//var rgb2 = new ColorRGB(hsl);
				//var rgb32 = new ColorRGB32Bit(rgb2);
				//var b = Color.FromArgb(rgb32.ToInt());



				var a = Color.FromArgb(255, 55, 122);
				var hsbk = a.ToHSBK();
				var b = hsbk.ToColor();
				if ((a.R != b.R) || (a.B != b.B) || (a.G != b.G))
				{
					Debug.WriteLine(i.ToString());
					Debug.WriteLine(a.ToString());
					Debug.WriteLine(b.ToString());

					new object();
				}

				Assert.AreEqual(a.R, b.R);
				Assert.AreEqual(a.G, b.G);
				Assert.AreEqual(a.B, b.B);

				new object();
			}
		}

		[TestMethod()]
		public void ToColorHSLTest()
		{
			Assert.Fail();
		}

		[TestMethod()]
		public void ToColorTest()
		{
			Assert.Fail();
		}
	}
}