using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TrackingService.Model.Objects {
	/// <summary>
	/// Used to sign a class as a tracking service object collection.
	/// </summary>
	public abstract class TrackingServiceCollection {
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }
		/// <summary>
		/// International Mobile Equipment Identity (basically, device identifier).
		/// </summary>
		public string Imei { get; set; }
	}
}