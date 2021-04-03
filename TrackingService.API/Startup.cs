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
using AspNetCore.Identity.Mongo;
using AspNetCore.Identity.Mongo.Model;
using TrackingService.API.Cache;
using TrackingService.Model.Objects;
using System.IO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using TrackingService.API.Database;
using System.Text.Json.Serialization;

namespace TrackingService.API {
	public class Startup {
		public IConfiguration Configuration { get; }
		/// <summary>
		/// Use Dependency Injection whenever you can. 
		/// This exists for when you really can't (ie. Decoder class).
		/// </summary>
		public static PositionCache PositionCache { get; private set; }

		public Startup(IConfiguration configuration) {
			Configuration = configuration;
		}

		public void ConfigureServices(IServiceCollection services) {
			services.AddControllersWithViews();

			// configure database
			var dbSettings = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("dbsettings.json", false, false)
				.Build()
				.GetSection(nameof(DatabaseSettings))
				.Get<DatabaseSettings>();
			services.AddDbContext<TrackingDbContext>(o => {
				o.UseNpgsql(dbSettings.ConnectionString);
			});

			services.AddSingleton<PositionCache>();

			services.AddRazorPages();
			services.AddSwaggerGen(c => {
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "TrackingService.API", Version = "v1" });
			});

			// identity
			services.AddIdentityCore<TrackingUser>(options => {
				options.User.RequireUniqueEmail = true;
				options.Password.RequireDigit = false;
				options.Password.RequireLowercase = false;
				options.Password.RequireUppercase = false;
				options.Password.RequireNonAlphanumeric = false;
			})
				.AddEntityFrameworkStores<TrackingDbContext>()
				.AddDefaultTokenProviders();

			// jwt
			var jwtConfig = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("jwt.json", false, false)
				.Build()
				.GetSection("JwtConfig")
				.Get<JwtConfig>();
			services.AddSingleton(jwtConfig);
			var key = Encoding.ASCII.GetBytes(jwtConfig.Secret);

			var tokenValidationParameters = new TokenValidationParameters {
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(key),
				ValidateIssuer = false,
				ValidateAudience = false,
				RequireExpirationTime = false,
				ValidateLifetime = true
			};
			services.AddSingleton(tokenValidationParameters);
			services.AddAuthentication(options => {
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(jwt => {
				jwt.SaveToken = true;
				jwt.TokenValidationParameters = tokenValidationParameters;				
			});
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime) {
			if (env.IsDevelopment()) {
				app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TrackingService.API v1"));
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints => {
				endpoints.MapControllers();
				endpoints.MapRazorPages();
			});

			app.UseCookiePolicy();

			// setup position cache
			PositionCache = app.ApplicationServices.GetRequiredService<PositionCache>();
			lifetime.ApplicationStopping.Register(OnShutdown);
		}

		private async void OnShutdown() {
			await PositionCache.ForcePersistPositions();
		}
	}
}
