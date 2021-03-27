using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackingService.API.Cache;
using TrackingService.API.Storage;
using TrackingService.Model.Objects;

namespace TrackingService.API {
	public class Startup {
		public IConfiguration Configuration { get; }
		public static PositionCache PositionCache { get; private set; }

		public Startup(IConfiguration configuration) {
			Configuration = configuration;
		}

		public void ConfigureServices(IServiceCollection services) {
			// configure database
			services.Configure<DatabaseSettings>(
				Configuration.GetSection(nameof(DatabaseSettings)));
			services.AddSingleton<IDatabaseSettings>(sp =>
				sp.GetRequiredService<IOptions<DatabaseSettings>>().Value);

			services.AddSingleton<PositionCache>();
			services.AddSingleton<PositionStore>();
			services.AddSingleton<DeviceStore>();

			services.AddControllers()
				.AddNewtonsoftJson(o => o.UseMemberCasing());
			services.AddSwaggerGen(c => {
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "TrackingService.API", Version = "v1" });
			});

		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime) {
			if (env.IsDevelopment()) {
				app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TrackingService.API v1"));
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints => {
				endpoints.MapControllers();
			});

			// setup position cache
			PositionCache = app.ApplicationServices.GetRequiredService<PositionCache>();
			lifetime.ApplicationStopping.Register(OnShutdown);
		}

		private async void OnShutdown() {
			await PositionCache.ForcePersistPositions();
		}
	}
}
