namespace TrackingService.Model.Objects {
	public enum AlarmKinds {
		None = -1,
		LowBattery = 0,
		GeofenceOut = 1,
		GeofenceIn = 2,
		Message = 3,
		SOS = 16,
		WatchTakeOff = 20
	}
}