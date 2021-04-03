using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using TrackingService.API.Database;
using TrackingService.Model.Objects;

namespace TrackingService.API.Cache {
	/// <summary>
	/// Stores positions in-memory for quick access and 
	/// persists them periodically in a database.
	/// </summary>
	public class PositionCache {
		private readonly IServiceScopeFactory _scopeFactory;
		private readonly ILogger<PositionCache> _logger;
		private readonly List<Position> _positions;
		private readonly List<string> _deviceWhitelist;
		private readonly List<string> _deviceBlacklist;
		private readonly Timer _timer;

		public delegate void PositionCacheEventHandler(object sender, PositionAddedEventArgs args);
		public event EventHandler<PositionAddedEventArgs> OnPositionRegistered;

		public PositionCache(ILogger<PositionCache> logger, IServiceScopeFactory scopeFactory) {
			_scopeFactory = scopeFactory;
			_logger = logger;
			_positions = new();
			_deviceWhitelist = new();
			_deviceBlacklist = new();

			_timer = new Timer() {
				AutoReset = true,
				Interval = TimeSpan.FromMinutes(0.3).TotalMilliseconds,
			};
			_timer.Elapsed += PersistPositionsEvent;
			_timer.Start();
		}

		private static TrackingDbContext GetContext(IServiceScope scope) => 
			scope.ServiceProvider.GetRequiredService<TrackingDbContext>();

		private async Task PersistPositions() {
			if (_positions.Count < 1) {
				return;
			}

			using (var scope = _scopeFactory.CreateScope()) {
				var db = GetContext(scope);
				if (_positions.Count == 1) {
					db.Positions.Add(_positions[0]);
				} else {
					await db.Positions.AddRangeAsync(_positions);
				}
				await db.SaveChangesAsync();
			}

			_positions.Clear();
		}

		private async void PersistPositionsEvent(object sender, ElapsedEventArgs e) {
			await PersistPositions();
		}

		public async Task ForcePersistPositions() {
			await PersistPositions();
		}

		/// <returns>Removal result.</returns>
		public bool RemoveDeviceFromBlacklist(string imei) {
			return _deviceBlacklist.Remove(imei);
		}

		public void Put(Position position) {
			if (_deviceBlacklist.Contains(position.Imei)) {
				return;
			}

			// do we know if this device is registered?
			if (!_deviceWhitelist.Contains(position.Imei)) {
				// check database, maybe it knows
				bool isDeviceRegistered;
				using (var scope = _scopeFactory.CreateScope()) {
					var db = GetContext(scope);
					isDeviceRegistered = db.Devices.AsNoTracking().Where(x => x.Imei == position.Imei).Any();
				}
				if (isDeviceRegistered) {
					// remember it so we don't have to query DB on each new position
					_deviceWhitelist.Add(position.Imei);					
				} else {
					// not registered, blacklist it (remember to remove it from list if it gets registered)
					_deviceBlacklist.Add(position.Imei);
					return;
				}
			}

			_positions.Add(position);
			RaiseOnPositionRegistered(new PositionAddedEventArgs(position.Imei));
		}

		private void RaiseOnPositionRegistered(PositionAddedEventArgs positionAddedEventArgs) {
			OnPositionRegistered?.Invoke(this, positionAddedEventArgs);
		}

		public Position Get(Position position) {
			return GetNewestByImei(position.Imei);
		}

		public Position GetNewestByImei(string imei) {
			var cachedPosition = _positions.Where(x => x.Imei == imei).OrderByDescending(x => x.Date).FirstOrDefault();

			if (cachedPosition is null) {
				using (var scope = _scopeFactory.CreateScope()) {
					var db = GetContext(scope);
					return db.Positions.Where(x => x.Imei == imei).OrderByDescending(x => x.Date).FirstOrDefault();
				}
			}

			return cachedPosition;
		}

		public async Task<List<Position>> GetListByImei(string imei, DateTime from, DateTime to) {
			// a list of positions shouldn't be cached; part of requested range may be outside 
			// cached positions, so we would end up checking database anyway
			using (var scope = _scopeFactory.CreateScope()) {
				var db = GetContext(scope);
				return await db.Positions.Where(x => x.Imei == imei && x.Date > from && x.Date < to).ToListAsync();
			}
		}
	}
}
