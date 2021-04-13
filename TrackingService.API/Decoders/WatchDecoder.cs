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
using TrackingService.API.Extensions;
using TrackingService.Model.Objects;

namespace TrackingService.API.Decoders {
	/// <summary>
	/// Watch is a protocol commonly used by cheap chinese watches or 
	/// animal GPS trackers. Very likely even Comarch's wristband.
	/// </summary>
	/// <remarks>
	/// Sample message: <c>[SG*8800000015*000D*LK,50,50,100]</c>
	/// </remarks>
	public class WatchDecoder : Decoder {
		private int ContentLength;
		private string Content;
		public Position Watch = new();
		public string Vendor;
		public string Imei;

		public override async Task<Position> DecodeAsync(string data) {
			if (!data.StartsWith("[") || !data.EndsWith("]")) {
				// discard, obviously bad message
				return null;
			}

			// remove "[" and "]"
			var message = data[1..^1];
			var tokenizedMessage = message.Split("*");

			Watch.Protocol = "watch";
			Vendor = tokenizedMessage[0];
			Imei = tokenizedMessage[1];
			Watch.Imei = tokenizedMessage[1];
			ContentLength = Convert.ToInt32(tokenizedMessage[2], 16); // length is a hex number
			Content = tokenizedMessage[3];
			var splitContent = Content.Split(",");
			string messageMode = splitContent[0];

			if (messageMode == "LK") {
				// LK = status update sent every few minutes, may contain steps, "rolling times" (?) and battery level
				if (splitContent.Length > 1) {
					Watch.MiscInfo.Steps = int.Parse(splitContent[1]);
					// index 2 - "rolling times"
					Watch.MiscInfo.Battery = splitContent[3] + "%";
				}
				await WatchRespond("LK");

			} else if (messageMode == "UD" || messageMode == "UD2" || messageMode == "AL") {
				// UD = location data
				// UD2 = blind spot location data
				// AL = alarm data
				// all look/work exactly the same
				var date = splitContent[1].InsertEvery(2, '.');
				var time = splitContent[2].InsertEvery(2, ':');
				Watch.Date = Convert.ToDateTime($"{date} {time}", new CultureInfo("en-GB"));
				var hasPosition = splitContent[3] == "A";

				// InvariantCulture needed to parse dot as a floating point
				var latitude = double.Parse(splitContent[4], CultureInfo.InvariantCulture);
				if (splitContent[5] == "S") latitude *= -1;
				var longitude = double.Parse(splitContent[6], CultureInfo.InvariantCulture);
				if (splitContent[7] == "W") longitude *= -1;
				Watch.Lat = latitude;
				Watch.Lon = longitude;

				Watch.Speed = double.Parse(splitContent[8], CultureInfo.InvariantCulture).ToKph();
				Watch.Direction = double.Parse(splitContent[9], CultureInfo.InvariantCulture);

				var alarmCode = int.Parse(splitContent[16]); // in protocol it's padded to four 0s, int.Parse removes it
				if (messageMode != "AL") alarmCode = -1;

				Watch.MiscInfo = new MiscInfo() {
					Alt = int.Parse(splitContent[10]),
					Satellites = int.Parse(splitContent[11]),
					SignalStrength = int.Parse(splitContent[12]),
					Battery = splitContent[13] + "%",
					Steps = int.Parse(splitContent[14]),
					Alarm = (AlarmKinds)alarmCode,
					Mcc = int.Parse(splitContent[19]),
					Mnc = int.Parse(splitContent[20])
				};

				if (messageMode == "AL") {
					await WatchRespond("AL");
				}

				// shouldn't respond unless in AL mode
			}

			return Watch;
		}

		private async Task WatchRespond(string content) {
			var hexLength = content.Length.ToString("X").PadLeft(4, '0');

			await Respond($"[{Vendor}*{Imei}*{hexLength}*{content}]");
		}
	}
}
