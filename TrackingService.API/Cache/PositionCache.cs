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
using TrackingService.Model.Objects.DataStructures;

namespace TrackingService.API.Cache {
	/// <summary>
	/// Stores positions in-memory for quick access and 
	/// persists them periodically in a database.
	/// </summary>
	public class PositionCache : DatabaseCache {
		private readonly IServiceScopeFactory _scopeFactory;
		private readonly ILogger<PositionCache> _logger;
		private readonly IndexedList<Position> _positions;
		private readonly DeviceCache _deviceCache;
		private readonly Timer _timer;

		public delegate void PositionCacheEventHandler(object sender, PositionAddedEventArgs args);
		public event EventHandler<PositionAddedEventArgs> OnPositionRegistered;

		public PositionCache(ILogger<PositionCache> logger, IServiceScopeFactory scopeFactory, DeviceCache deviceCache) {
			_scopeFactory = scopeFactory;
			_logger = logger;
			_positions = new();
			_deviceCache = deviceCache;

			_timer = new Timer() {
				AutoReset = true,
				Interval = TimeSpan.FromMinutes(5).TotalMilliseconds,
			};
			_timer.Elapsed += PersistPositionsEvent;
			_timer.Start();
		}

		private async Task PersistPositions() {
			if (_positions.Count() < 1) {
				return;
			}

			using (var scope = _scopeFactory.CreateScope()) {
				var db = GetContext(scope);
				await db.Positions.AddRangeAsync(_positions.Flattened());
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

		public void Add(Position position) {
			if (!_deviceCache.DeviceExists(position.Imei, out _)) {
				return;
			}

			_positions.Add(position.Imei, position);
			RaiseOnPositionRegistered(new PositionAddedEventArgs(position.Imei));
		}

		private void RaiseOnPositionRegistered(PositionAddedEventArgs positionAddedEventArgs) {
			OnPositionRegistered?.Invoke(this, positionAddedEventArgs);
		}

		public Position GetNewestByImei(string imei) {
			var cachedPosition = _positions.GetAll(imei).OrderByDescending(x => x.Date).FirstOrDefault();

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
