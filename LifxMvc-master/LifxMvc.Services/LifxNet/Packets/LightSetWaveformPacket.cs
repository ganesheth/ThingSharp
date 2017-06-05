using LifxMvc.Domain;
using LifxNet.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxNet
{
	/// <summary>
	/// Sent by a client to cause the light to display a configurable flashing
	/// pattern, such as used by the "pulse" and "breathe" effects in the HTTP
	/// API.
	/// </summary>
	public class LightSetWaveformPacket : LifxPacketBase
	{
		byte Reserved { get; set; } //| unsigned 8-bit integer                                |

		/// <summary>
		/// If Transient is true, the color returns to the original color of the
		/// light, after the specified number of Cycles. If Transient is false,
		/// the light is left set to Color after the specified number of Cycles.
		/// </summary>
		public bool Transient { get; set; } //| unsigned 8-bit integer, interpreted as boolean        |
		public IHSBK Color { get; set; } //| HSBK                                                  |

		/// <summary>
		/// The Period is the length of one cycle in milliseconds.
		/// </summary>
		public UInt32 Period { get; set; } //| unsigned 32-bit integer                               |

		public Single Cycles { get; set; } //| 32-bit float                                          |

		/// <summary>
		/// If DutyCycle is 0, an equal amount of time is spent on the original
		/// color and the new Color.If DutyCycle is positive, more time is
		/// spent on the original color. If DutyCycle is negative, more time
		/// is spent on the new color.
		/// </summary>
		public Int16 DutyCycle { get; set; } //| signed 16-bit integer                                 |

		/// <summary>
		/// Describes the type of flashing pattern used in the
		/// [SetWaveform](#setwaveform---103) message.
		/// </summary>
		public WaveformEnum Waveform { get; set; } // unsigned 8-bit integer, maps to[Waveform](#waveform) |

		public override PacketType MessageType { get { return PacketType.LightSetWaveform; } }

		public LightSetWaveformPacket(IBulb bulb, LightSetWaveformCreationContext ctx) : base(bulb)
		{
			this.Header.ResponseRequired = true;

			this.Transient = ctx.Transient;
			this.Color = ctx.Color;
			this.Period = ctx.Period;
			this.Cycles = ctx.Cycles;
			this.DutyCycle = ctx.DutyCycle;
			this.Waveform = ctx.Waveform;
		}

		public LightSetWaveformPacket(FrameHeader header, byte[] payload)
			: base(header)
		{
			this.Reserved = payload[0];
			this.Transient = payload[1] > 0;

			var h = BitConverter.ToUInt16(payload, 2);
			var s = BitConverter.ToUInt16(payload, 4);
			var b = BitConverter.ToUInt16(payload, 6);
			var k = BitConverter.ToUInt16(payload, 8);
			this.Color = HSBK.Create(h, s, b, k);

			this.Period = BitConverter.ToUInt16(payload, 10);
			this.Cycles = BitConverter.ToSingle(payload, 12);
			this.DutyCycle = BitConverter.ToInt16(payload, 14);
			this.Waveform = (WaveformEnum)payload[16];
		}



		override protected object[] GetPayloadParams()
		{
			var result = new object[]
			{
				this.Reserved,
				(byte) (this.Transient ? 1: 0),

				this.Color.Hue,
				this.Color.Saturation,
				this.Color.Brightness,
				this.Color.Kelvin,

				this.Period,
				this.Cycles,
				this.DutyCycle,
				(byte)this.Waveform
			};

			return result;
		}


	}//class
}//ns
