using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackingService.API.Storage;
using TrackingService.Model.Objects;

namespace TrackingService.API.Cache {
	/// <summary>
	/// Stores positions in-memory for quick access and persists them
	/// in a database.
	/// </summary>
	public class PositionCache {
		private readonly ILogger<PositionCache> _logger;
		private readonly DeviceStore _deviceStore;
		private readonly PositionStore _positionStore;
		private readonly List<Position> _positions;

		public PositionCache(DeviceStore deviceStore, PositionStore positionStore, ILogger<PositionCache> logger) {
			_logger = logger;
			_deviceStore = deviceStore;
			_positionStore = positionStore;
			_positions = new List<Position>();
		}

		public void Put(Position position) {
			var deviceExists = _deviceStore.GetByImei(position.Imei) != null;

			if (deviceExists) {
				_logger.LogInformation($"Position for device {position.Imei} should be cached now.");
			} else {
				_logger.LogInformation($"Ignoring caching of {position.Imei} - device not registered.");
			}

		}
	}
}
