using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TrackingService.API.Decoders;

namespace TrackingService.API {
	/// <summary>
	/// Decides which decoder should be chosen to a given port and 
	///	passes the connection to it.
	/// </summary>
	internal class DecoderSelector : ConnectionHandler {

		private readonly ILogger<DecoderSelector> Logger;
		private readonly List<KeyValuePair<string, string>> Configuration;

		public DecoderSelector(ILogger<DecoderSelector> logger) {
			Logger = logger;
			Configuration = new ConfigurationBuilder()
							.SetBasePath(Directory.GetCurrentDirectory())
							.AddJsonFile("ports.json", false, false)
							.Build()
							.AsEnumerable()
							.ToList();
		}

		public override async Task OnConnectedAsync(ConnectionContext connection) {
			var port = new Uri("http://" + connection.LocalEndPoint.ToString()).Port;
			var protocol = Configuration.Where(x => x.Value == port.ToString())
										.Select(x => x.Key)
										.FirstOrDefault();

			var transport = connection.Transport;

			switch (protocol) {
				case "json":
					await new JsonDecoder().ReceiveAsync(transport);
					break;
				case "watch":
					await new WatchDecoder().ReceiveAsync(transport);
					break;
				case "gt06":
					await new Gt06Decoder().ReceiveAsync(transport);
					break;
				default:
					throw new NullReferenceException($"Couldn't determine protocol decoder for \"{protocol}\".");
			}
		}
	}
}