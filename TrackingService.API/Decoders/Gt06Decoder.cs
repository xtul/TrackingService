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
using TrackingService.API.Cache;
using TrackingService.Model.Objects;

namespace TrackingService.API.Decoders {
	/// <summary>
	/// Protocol commonly used by Concox devices. A lot of knock-off devices
	/// also use this protocol, albeit often make off steps from the standard.
	/// </summary>
	public class Gt06Decoder : Decoder {
		public override Task<Position> DecodeAsync(string data) {
			throw new NotImplementedException();
		}
	}
}
