using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrackingService.API.Cache {
	public class PositionAddedEventArgs : EventArgs {
		public string Imei { get; set; }
		
		public PositionAddedEventArgs(string imei) {
			Imei = imei;
		}
	}
}
