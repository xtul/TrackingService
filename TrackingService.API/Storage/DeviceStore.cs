using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackingService.Model.Objects;

namespace TrackingService.API.Storage {
	public class DeviceStore : DefaultStore<Device> {
		public DeviceStore(IDatabaseSettings settings) : base(settings) {
		}
	}
}
