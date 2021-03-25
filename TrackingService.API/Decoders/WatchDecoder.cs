using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TrackingService.API.Decoders.DecodedObjects;
using TrackingService.Model.Objects;

namespace TrackingService.API.Decoders {
	public class WatchDecoder : Decoder {
		private string Vendor;
		private string Imei;
		private int ContentLength;
		private string Content;
		private int Steps;
		private int Battery;

		protected override async Task DecodeAsync() {
			if (!Data.StartsWith("[") || !Data.EndsWith("]")) {
				// obviously bad message
				return;
			}

			// remove "[" and "]"
			var message = Data[1..^1];
			var tokenizedMessage = message.Split("*");

			Vendor = tokenizedMessage[0];
			Imei = tokenizedMessage[1];
			ContentLength = Convert.ToInt32(tokenizedMessage[2], 16); // length is a hex number
			Content = tokenizedMessage[3];
			var splitContent = Content.Split(",");
			string messageMode = splitContent[0];

			if (messageMode == "LK") {
				// LK = status update sent every few minutes, may contain steps, "rolling times" (?) and battery level
				if (splitContent.Length > 1) {
					_ = int.TryParse(splitContent[1], out Steps);
					// index 2 - "rolling times"
					_ = int.TryParse(splitContent[3], out Battery);
				}
				await WatchRespond("LK");
			} else if (messageMode == "UD" || messageMode == "UD2" || messageMode == "AL") {
				// UD = location data
				// UD2 = blind spot location data (last known spot? LBS location? looks exactly the same)
				// AL = alarm data (looks exactly the same as UD/UD2)
				throw new NotImplementedException();
			} else if (messageMode == "WAD" || messageMode == "WG") {
				// WAD = address request
				// WG = hard to tell, documentation says "Request instruction of latitude and longitude"
				// I may avoid implementing it altogether
				throw new NotImplementedException();
			}

			var result = new Watch {
				Vendor = Vendor,
				Imei = Imei,
				Steps = Steps,
				Battery = Battery
			};

			Console.WriteLine(result.ToString());
		}

		private async Task WatchRespond(string content) {
			var hexLength = ContentLength.ToString("X").PadLeft(4, '0');

			await Respond($"[{Vendor}*{Imei}*{hexLength}*{content}]");
		}
	}
}
