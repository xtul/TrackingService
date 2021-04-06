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
		/// <summary>
		/// Longitude.
		/// </summary>
		public float Lon { get; set; }
		/// <summary>
		/// Latitude.
		/// </summary>
		public float Lat { get; set; }
		public DateTime Date { get; set; }
		private double _speed;
		public double Speed {
			get => Math.Round(_speed, 1, MidpointRounding.ToZero);
			set => _speed = value;
		}
		private double _direction;
		public double Direction {
			get => Math.Round(_direction, 2, MidpointRounding.ToZero);
			set => _direction = value % 360f;
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
