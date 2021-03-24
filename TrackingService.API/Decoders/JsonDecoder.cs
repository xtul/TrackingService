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
	public class JsonDecoder : Decoder {
		protected override async Task DecodeAsync() {
			var json = JsonConvert.DeserializeObject<JsonMessage>(Data);

			if (json != null) {
				await Respond(json.Imei);
			} else {
				await Respond("0");
			}
		}
	}
}
