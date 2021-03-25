using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TrackingService.Model.Objects {
	public class Position : TrackingServiceCollection {
		public string Imei { get; set; }
		public float Lon { get; set; }
		public float Lat { get; set; }
		public DateTime Time  { get; set; }
	}
}
