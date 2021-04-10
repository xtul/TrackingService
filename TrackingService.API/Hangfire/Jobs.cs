using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackingService.API.Database;
using TrackingService.Model.Objects;

namespace TrackingService.API.Hangfire {
	public class Jobs {
		private readonly TrackingDbContext _context;
		private readonly ILogger<Jobs> _logger;
		private readonly DatabaseSettings _dbsettings;

		public Jobs(TrackingDbContext context, ILogger<Jobs> logger, DatabaseSettings dbsettings) {
			_context = context;
			_logger = logger;
			_dbsettings = dbsettings;
		}

		public async Task RemoveDeadRefreshTokens() {
			var tokensToDelete = _context.RefreshTokens.Where(x => x.IsRevoked || x.ExpiryDate < DateTime.UtcNow).ToArray();
			if (tokensToDelete is not null && tokensToDelete.Length > 0) {
				_context.RefreshTokens.RemoveRange(tokensToDelete);
				await _context.SaveChangesAsync();

				_logger.LogInformation($"Removed {tokensToDelete.Length} refresh tokens.");
			}
		}

		public async Task RemoveOldPositions() {
			var daysToKeep = _dbsettings.PositionDaysToKeep * -1;
			var maxDateToKeep = DateTime.Now.AddDays(daysToKeep);

			var positionsToDelete = _context.Positions.Where(x => x.Date < maxDateToKeep).ToArray();
			if (positionsToDelete is not null && positionsToDelete.Length > 0) {
				_context.Positions.RemoveRange(positionsToDelete);
				await _context.SaveChangesAsync();

				_logger.LogInformation($"Removed {positionsToDelete.Length} positions (older than {maxDateToKeep}.)");
			}
		}
	}
}
