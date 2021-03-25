using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrackingService.API.Decoders.DecodedObjects {
	public class Watch {
		public string Vendor { get; set; }
		public string Imei { get; set; }
		public int Steps { get; set; }
		public int Battery { get; set; }

		public override string ToString() {
			return			$"Received Watch:\n" +
							$"-------------------------------------\n" +
							$"Vendor: {Vendor}\n" +
							$"IMEI: {Imei}\n" +
							$"Steps: {Steps}\n" +
							$"Battery: {Battery}\n" +
							$"-------------------------------------";
		}
	}
}
