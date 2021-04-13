using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackingService.Model.Objects;

namespace TrackingService.API.Cache {
	public class PositionAddedEventArgs : EventArgs {
		public string Imei { get; set; }
		public Position Position { get; set; }
		
		public PositionAddedEventArgs(string imei, Position position) {
			Imei = imei;
			Position = position;
		}
	}
}
