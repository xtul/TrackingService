using static Microsoft.AspNetCore.Connections.ConnectionBuilderExtensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace TrackingService.API {
	public class Program {
		public static void Main(string[] args) {
			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder => {
					webBuilder.ConfigureKestrel(o => {
						var config = new ConfigurationBuilder()
							.SetBasePath(Directory.GetCurrentDirectory())
							.AddJsonFile("ports.json", false, false)
							.AddCommandLine(args)
							.Build()
							.AsEnumerable();

						foreach (var kv in config) {
							if (kv.Key == "http" || kv.Key == "https") {
								continue;
							}

							var port = int.Parse(kv.Value);
							o.ListenLocalhost(port, builder => {
								builder.UseConnectionHandler<DecoderSelector>();
								builder.Protocols = HttpProtocols.None;
							});
						}

						var http = int.Parse(config.Where(x => x.Key == "http").Select(x => x.Value).FirstOrDefault());
						var https = int.Parse(config.Where(x => x.Key == "https").Select(x => x.Value).FirstOrDefault());

						o.ListenLocalhost(http);
						o.ListenLocalhost(https, builder =>
						{
							builder.UseHttps();
						});

					});
					webBuilder.UseStartup<Startup>();
				});
	}
}
