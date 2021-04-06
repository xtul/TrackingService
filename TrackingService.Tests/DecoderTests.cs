using System;
using System.Threading.Tasks;
using TrackingService.API.Decoders;
using TrackingService.Model.Objects;
using Xunit;

namespace TrackingService.Tests {
	public class DecoderTests {
		[Fact]
		public async Task WatchTest() {
			var message = "[SG*8800000015*0087*AL,220414,134652,A,22.571707,N,113.8613968,E,0.1,0.0,100,7,60,90,1000,50,0000,4,1,460,0,9360,4082,131,9360,4092,148,9360,4091,143,9360,4153,141]";
			var decoder = new WatchDecoder();

			var position = await decoder.DecodeAsync(message);

			Assert.Equal("SG", decoder.Vendor);
			Assert.Equal("8800000015", position.Imei);
			Assert.Equal(new DateTime(2014,4,22,13,46,52), position.Date);
			Assert.Equal(22.571707, position.Lat);
			Assert.Equal(-113.861397, position.Lon); // should round "away from zero", despite GPS tracker reporting above 6 decimal places
			Assert.Equal(0.1d, position.Speed);
			Assert.Equal(0.0d, position.Direction);
			Assert.Equal(100, position.MiscInfo.Alt);
			Assert.Equal(7, position.MiscInfo.Satellites);
			Assert.Equal(60, position.MiscInfo.SignalStrength);
			Assert.Equal("90%", position.MiscInfo.Battery);
			Assert.Equal(1000, position.MiscInfo.Steps);
			Assert.Equal(AlarmKinds.LowBattery, position.MiscInfo.Alarm);
			Assert.Equal(460, position.MiscInfo.Mcc);
			Assert.Equal(0, position.MiscInfo.Mnc);
		}
	}
}
