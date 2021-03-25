using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TrackingService.Model.Objects;

namespace TrackingService.API.Decoders {
	/// <summary>
	/// This decoder is meant to be used by a (NYI) mobile application.
	/// </summary>
	public class JsonDecoder : Decoder {
		protected override async Task<Position> DecodeAsync(string data) {
			var json = JsonConvert.DeserializeObject<JsonMessage>(data);

			if (json != null) {
				await Respond(json.Imei);
			} else {
				await Respond("0");
			}

			return null;
		}
	}
}
