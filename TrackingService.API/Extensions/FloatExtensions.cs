using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrackingService.API.Extensions {
	public static class FloatExtensions {
		/// <summary>
		/// Converts kilometers per hour to miles per hour.
		/// </summary>
		public static double ToKph(this double miles) {
			return Math.Round(miles * 1.609344, 1, MidpointRounding.ToZero);
		}

		/// <summary>
		/// Converts miles per hour to kilometers per hour.
		/// </summary>
		public static double ToMph(this double kilometers) {
			return Math.Round(kilometers * 0.6213712, 1, MidpointRounding.ToZero);
		}
	}
}
