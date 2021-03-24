using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackingService.API.Storage;

namespace TrackingService.API.Controllers {
	public class CacheController : ControllerBase {
		private readonly DeviceStore _deviceStore;
		private readonly PositionStore _positionStore;

		public CacheController(DeviceStore deviceStore, PositionStore positionStore) {
			_deviceStore = deviceStore;
			_positionStore = positionStore;
		}
	}
}
