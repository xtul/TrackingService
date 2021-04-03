using Newtonsoft.Json;
using System.Threading.Tasks;
using TrackingService.API.Cache;
using TrackingService.Model.Objects;

namespace TrackingService.API.Decoders {
	/// <summary>
	/// This decoder is meant to be used by a (NYI) mobile application.
	/// </summary>
	public class JsonDecoder : Decoder {
		protected override Task<Position> DecodeAsync(string data) {
			var position = JsonConvert.DeserializeObject<Position>(data);

			if (position is null) {
				return null;
			}

			return Task.FromResult(position);
		}
	}
}
