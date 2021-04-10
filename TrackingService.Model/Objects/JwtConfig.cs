namespace TrackingService.API {
	public class JwtConfig {
		public string Secret { get; set; }
		public string SigningKey { get; set; }
		public int SecondsLifespan { get; set; }
	}
}