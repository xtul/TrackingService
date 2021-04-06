using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackingService.Model.Objects.DbSet {
	public class UserDevice {
		public int UserId { get; set; }
		public int DeviceId { get; set; }

		[ForeignKey("UserId")]
		public TrackingUser User { get; set; }
		[ForeignKey("DeviceId")]
		public Device Device { get; set; }
	}
}
