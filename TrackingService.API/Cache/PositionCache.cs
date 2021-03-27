using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using TrackingService.API.Storage;
using TrackingService.Model.Objects;

namespace TrackingService.API.Cache {
	/// <summary>
	/// Stores positions in-memory for quick access and 
	/// persists them periodically in a database.
	/// </summary>
	public class PositionCache {
		private readonly ILogger<PositionCache> _logger;
		private readonly DeviceStore _deviceStore;
		private readonly PositionStore _positionStore;
		private readonly HashSet<Position> _positions;
		private readonly Timer _timer;

		public delegate void PositionCacheEventHandler(object sender, PositionAddedEventArgs args);
		public event EventHandler<PositionAddedEventArgs> OnPositionRegistered;

		public PositionCache(DeviceStore deviceStore, PositionStore positionStore, ILogger<PositionCache> logger) {
			_logger = logger;
			_deviceStore = deviceStore;
			_positionStore = positionStore;
			_positions = new HashSet<Position>();

			_timer = new Timer() {
				AutoReset = true,
				Interval = TimeSpan.FromMinutes(5).Milliseconds,
			};
			_timer.Elapsed += PersistPositionsEvent;
			_timer.Start();
		}

		protected async Task PersistPositions() {
			await _positionStore.Create(_positions);

			_positions.Clear();
		}

		private async void PersistPositionsEvent(object sender, ElapsedEventArgs e) {
			await PersistPositions();
		}

		public async Task ForcePersistPositions() {
			await PersistPositions();
		}

		public void Put(Position position) {
			var isDeviceRegistered = _deviceStore.GetByImei(position.Imei) != null;

			if (isDeviceRegistered) {
				_positions.Add(position);
			}

			RaiseOnPositionRegistered(new PositionAddedEventArgs(position.Imei));
		}

		private void RaiseOnPositionRegistered(PositionAddedEventArgs positionAddedEventArgs) {
			OnPositionRegistered?.Invoke(this, positionAddedEventArgs);
		}

		public Position GetById(string id) {
			var cachedPosition = _positions.Where(x => x.Id == id).FirstOrDefault();

			if (cachedPosition is null) {
				return _positionStore.GetById(id);
			}

			return cachedPosition;
		}

		public Position GetByImei(string imei) {
			var cachedPosition = _positions.Where(x => x.Imei == imei).FirstOrDefault();

			if (cachedPosition is null) {
				return _positionStore.GetByImei(imei);
			}

			return cachedPosition;
		}
	}
}
