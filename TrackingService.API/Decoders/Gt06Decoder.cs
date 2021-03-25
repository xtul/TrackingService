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
	/// Protocol commonly used by Concox devices. A lot of knock-off devices
	/// also use this protocol, albeit often make off steps from the standard.
	/// </summary>
	public class Gt06Decoder : Decoder {
		protected override Task DecodeAsync() {
			throw new NotImplementedException();
		}
	}
}
