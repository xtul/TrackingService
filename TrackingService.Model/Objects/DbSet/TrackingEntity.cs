using System.ComponentModel.DataAnnotations;

namespace TrackingService.Model.Objects {
	/// <summary>
	/// Used to sign a class as a tracking service object collection.
	/// </summary>
	public abstract class TrackingEntity {
		[Key]
		public int Id { get; set; }
		/// <summary>
		/// International Mobile Equipment Identity (basically, device identifier).
		/// </summary>
		public string Imei { get; set; }
	}
}