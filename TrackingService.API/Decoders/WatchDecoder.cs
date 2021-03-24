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
	public class WatchDecoder : Decoder {
		protected override async Task DecodeAsync() {
			if (!string.IsNullOrEmpty(Data)) {
				Console.WriteLine(@"Received data:" + "\n" +
									"--------------------------------\n" +
									Data + "\n" +
									"--------------------------------\n" +
									"\n");
			}

			await Respond(Data);
		}
	}
}
