using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace TrackingService.Model.Objects {
	public class Device : TrackingCollection {
		public string Imei { get; set; }
		public string Name { get; set; }
		public float Lon { get; set; }
		public float Lat { get; set; }
		public DateTime LastUpdate { get; set; }
	}
}
