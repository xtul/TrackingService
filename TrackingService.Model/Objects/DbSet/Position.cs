using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace TrackingService.Model.Objects {
	public class Position : TrackingEntity {
		public string Protocol { get; set; }
		private double _lon;
		/// <summary>
		/// Longitude.
		/// </summary>
		public double Lon {
			get => Math.Round(_lon, 6, MidpointRounding.AwayFromZero);
			set => _lon = Math.Round(value, 6, MidpointRounding.AwayFromZero);
		}
		private double _lat;
		/// <summary>
		/// Latitude.
		/// </summary>
		public double Lat {
			get => Math.Round(_lat, 6, MidpointRounding.AwayFromZero);
			set => _lat = Math.Round(value, 6, MidpointRounding.AwayFromZero);
		}
		public DateTime Date { get; set; }
		private double _speed;
		public double Speed {
			get => Math.Round(_speed, 2, MidpointRounding.AwayFromZero);
			set => _speed = Math.Round(value, 2, MidpointRounding.AwayFromZero);
		}
		private double _direction;
		public double Direction {
			get => Math.Round(_direction, 2, MidpointRounding.AwayFromZero);
			set => _direction = Math.Round(value % 360d, 2, MidpointRounding.AwayFromZero);
		}
		private string _miscInfo;
		public MiscInfo MiscInfo {
			get {
				return JsonConvert.DeserializeObject<MiscInfo>(string.IsNullOrEmpty(_miscInfo) ? "{}" : _miscInfo);
			}
			set {
				try {
					_miscInfo = JsonConvert.SerializeObject(value);
				} catch (NullReferenceException) {
					_miscInfo = "{}";
				}
			}
		}

		public Position() {
			if (MiscInfo is null) {
				MiscInfo = new();
			}
		}

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
