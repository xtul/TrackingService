using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackingService.API.Storage;
using TrackingService.Model.Objects;

namespace TrackingService.API.Cache {
	public class PositionCache {
		private readonly DeviceStore _deviceStore;
		private readonly PositionStore _positionStore;
		public readonly List<Position> _positions;

		public PositionCache(DeviceStore deviceStore, PositionStore positionStore) {
			_deviceStore = deviceStore;
			_positionStore = positionStore;
			_positions = new List<Position>();
		}


	}
}
