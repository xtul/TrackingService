using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackingService.Model.Objects;

namespace TrackingService.API.Storage {
	public class PositionStore : DefaultStore<Position> {
		public PositionStore(IDatabaseSettings settings) : base(settings) {
		}
	}
}
