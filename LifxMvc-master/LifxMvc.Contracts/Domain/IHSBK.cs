namespace LifxMvc.Domain
{
	public interface IHSBK
	{
		ushort Brightness { get; }
		ushort Hue { get; }
		ushort Kelvin { get; }
		ushort Saturation { get; }

		bool IsColor { get; }
		void GetHSB(out double h, out double s, out double b);

	}
}