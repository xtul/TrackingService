using System;

namespace TrackingService.Model.Objects {
	/// <summary>
	/// Other position-specific information. Not essential.
	/// </summary>
	public class MiscInfo {
		/// <summary>
		/// Altitude.
		/// </summary>
		public float Alt { get; set; }
		public int Satellites { get; set; }
		private int _signalStrength;
		public int SignalStrength {
			get => _signalStrength;
			set => _signalStrength = Math.Clamp(value, 0, 100);
		}
		public string Battery { get; set; } // may have different formats - voltage, percentage...
		public int Steps { get; set; }
		public AlarmKinds Alarm { get; set; } = AlarmKinds.None;
		public int Mcc { get; set; }
		public int Mnc { get; set; }
	}
}