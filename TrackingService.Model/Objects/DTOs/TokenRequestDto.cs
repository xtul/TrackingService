using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackingService.Model.Objects.DTOs {
	public record TokenRequestDto {
		[Required]
		public string Token { get; set; }
		[Required]
		public string RefreshToken { get; set; }
	}
}
