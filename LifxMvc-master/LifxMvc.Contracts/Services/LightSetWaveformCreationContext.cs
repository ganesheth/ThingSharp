using LifxMvc.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxNet.Domain
{
	public enum WaveformEnum
	{
		Saw = 0,
		Sine = 1,
		HalfSine = 2,
		Triangle = 3,
		Pulse = 4
	};

	public class LightSetWaveformCreationContext
	{
		public bool Transient { get; set; } //| unsigned 8-bit integer, interpreted as boolean        |
		public IHSBK Color { get; set; } //| HSBK                                                  |
		public UInt32 Period { get; set; } //| unsigned 32-bit integer                               |
		public Single Cycles { get; set; } //| 32-bit float                                          |
		public Int16 DutyCycle { get; set; } //| signed 16-bit integer                                 |
		public WaveformEnum Waveform { get; set; } // unsigned 8-bit integer, maps to[Waveform](#waveform) |

		/// <summary>
		/// 
		/// </summary>
		/// <param name="transient">If Transient is true, the color returns to the original color of the
		/// light, after the specified number of Cycles. If Transient is false,
		/// the light is left set to referenced HSBK after the specified number of Cycles.
		/// </param>
		/// <param name="hsbk"></param>
		/// <param name="period">
		/// The Period is the length of one cycle in milliseconds.
		/// </param>
		/// <param name="cycles">
		/// The number of cycles
		/// </param>
		/// <param name="dutyCycle">
		/// If DutyCycle is 0, an equal amount of time is spent on the original
		/// color and the new Color.If DutyCycle is positive, more time is
		/// spent on the original color. If DutyCycle is negative, more time
		/// is spent on the new color.
		/// </param>
		/// <param name="waveform">
		/// Describes the type of flashing pattern used in the
		/// [SetWaveform](#setwaveform---103) message.
		/// </param>
		public LightSetWaveformCreationContext(bool transient, IHSBK hsbk, UInt32 period, Single cycles, Int16 dutyCycle, WaveformEnum waveform)
		{
			this.Transient = transient;
			this.Color = hsbk;
			this.Period = period;
			this.Cycles = cycles;
			this.DutyCycle = dutyCycle;
			this.Waveform = waveform;
		}
	}

}
