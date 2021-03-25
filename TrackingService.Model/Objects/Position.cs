using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TrackingService.Model.Objects {
	public class Position : TrackingServiceCollection {
		public string Protocol { get; set; }
		/// <summary>
		/// Longitude.
		/// </summary>
		public float Lon { get; set; }
		/// <summary>
		/// Latitude.
		/// </summary>
		public float Lat { get; set; }
		public DateTime Date { get; set; }
		public float Speed { get; set; }
		private float _direction;

		public Position() {
			MiscInfo = new MiscInfo();
		}

		public float Direction {
			get => _direction;
			set => _direction = value % 360f;
		}
		public MiscInfo MiscInfo { get; set; }

		public override string ToString() {
			return $"-------------------------------------\n" +
					$"Protocol: {Protocol}\n" +
					$"IMEI: {Imei}\n" +
					$"Longitude: {Lon}\n" +
					$"Latitude: {Lat}\n" +
					$"Date: {Date}\n" +
					$"Speed: {Speed}\n" +
					$"Direction: {Direction}\n" +
					$"    Altitude: {MiscInfo.Alt}\n" +
					$"    Satellites: {MiscInfo.Satellites}\n" +
					$"    Signal strength: {MiscInfo.SignalStrength}\n" +
					$"    Battery: {MiscInfo.Battery}\n" +
					$"    Steps: {MiscInfo.Steps}\n" +
					$"    Alarm: {MiscInfo.Alarm}\n" +
					$"    MCC: {MiscInfo.Mcc}\n" +
					$"    MNC: {MiscInfo.Mnc}\n" +
					$"-------------------------------------";
		}
	}
}
