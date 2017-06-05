using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxMvc.Domain
{
	public enum LifxProductEnum
	{
		Unknown = 0,
		Original1000 = 1,
		Color650 = 3,
		White800_LowVoltage = 10,
		White800_HighVoltage = 11,
		White900_BR30_LowVoltage = 18,
		Color1000_BR30 = 20,
		Color1000 = 22
	};

	public static class Extensions
	{
		public static bool IsColor(this LifxProductEnum product)
		{
			bool result = false;

			if (LifxProductEnum.Color1000 == product
				|| LifxProductEnum.Color1000_BR30 == product
				|| LifxProductEnum.Color650 == product
				|| LifxProductEnum.Original1000 == product)
				result = true;

			return result;
		}
	}
}
