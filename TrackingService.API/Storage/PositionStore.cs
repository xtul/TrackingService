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

		public async Task<List<Position>> GetPositionList(string imei, DateTime from, DateTime to) {
			var results = await _mongoCollection.FindAsync(x =>
				x.Date > from && 
				x.Date < to && 
				x.Imei == imei);
			return await results.ToListAsync();
		}
	}
}
