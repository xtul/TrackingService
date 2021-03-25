using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace TrackingService.Model.Objects {
	public class Device : TrackingServiceCollection {
		public string Name { get; set; }
	}
}
