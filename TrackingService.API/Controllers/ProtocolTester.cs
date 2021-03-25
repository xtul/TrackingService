using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Sockets;
using System.Text;
using TrackingService.Model.Objects;

namespace TrackingService.API.Controllers {
	[Route("api/testing")]
	[ApiController]
	public class ProtocolTester : ControllerBase {
		private readonly ILogger<ProtocolTester> Logger;

		public ProtocolTester(ILogger<ProtocolTester> logger) {
			Logger = logger;
		}

		private static string GetResponse(string message, int port) {
			var client = new TcpClient();

			client.Connect("127.0.0.1", port);

			// send a message to the connection's stream
			var buffer = Encoding.ASCII.GetBytes(message);
			var stream = client.GetStream();
			stream.Write(buffer, 0, buffer.Length);

			// receive response
			buffer = new byte[256];
			var bytes = stream.Read(buffer, 0, buffer.Length);
			var response = Encoding.ASCII.GetString(buffer, 0, bytes);

			// close the stream and connection
			stream.Close();
			client.Close();

			return response;
		}

		[HttpPost]
		[Route("json")]
		public string PostJson([FromBody] JsonMessage json) {
			var message = JsonConvert.SerializeObject(json);

			return GetResponse(message, 6001);
		}

		[HttpPost]
		[Route("watch")]
		public string PostWatch([FromBody] ProtocolTestingMessage message) {
			return GetResponse(message.Message, 6002);
		}
	}
}
