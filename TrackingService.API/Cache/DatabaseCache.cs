using Microsoft.Extensions.DependencyInjection;
using TrackingService.API.Database;

namespace TrackingService.API.Cache {
	public abstract class DatabaseCache {
		protected static TrackingDbContext GetContext(IServiceScope scope) =>
			scope.ServiceProvider.GetRequiredService<TrackingDbContext>();
	}
}