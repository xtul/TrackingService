using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrackingService.API.Extensions {
	public static class FloatExtensions {
		/// <summary>
		/// Converts kilometers per hour to miles per hour.
		/// </summary>
		public static float ToKph(this float miles) {
			return (float)(miles * 1.609344);
		}

		/// <summary>
		/// Converts miles per hour to kilometers per hour.
		/// </summary>
		public static float ToMph(this float kilometers) {
			return (float)(kilometers * 0.6213712);
		}
	}
}
