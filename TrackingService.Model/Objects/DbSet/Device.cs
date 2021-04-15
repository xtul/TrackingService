using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace TrackingService.Model.Objects {
	public class Device : TrackingEntity {
		public string Name { get; set; }
		public bool Enabled { get; set; } = true;

		public void CopyValues(Device newValues) {
			Name = newValues.Name;
			Imei = newValues.Imei;
			Enabled = newValues.Enabled;
		}
	}
}
