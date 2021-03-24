using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrackingService.Model.Objects {
	/// <summary>
	/// A JSON-encoded message.
	/// </summary>
	public class JsonMessage {
		public string Imei { get; set; }
		public float Lon { get; set; }
		public float Lat { get; set; }
	}
}
