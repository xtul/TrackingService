# TrackingService
An experimental, work-in-progress GPS tracking service. For web interface, [see this repository](https://github.com/xtul/tracking-web).

## Usage

TrackingService is not yet usable. Currently the only GPS tracking protocol implemented is `watch`. It's usually used by mobile GPS trackers based on MTK6261 CPU. It is also highly possible that Comarch's wristband uses this protocol - all cheap, chinese wrist devices have common features, battery size (~300mAH) or even shape.

You can also send position data as JSON - see [Position.cs](https://github.com/xtul/TrackingService/blob/master/TrackingService.Model/Objects/DbSet/Position.cs). By default, JSON decoder listens on port 6001.

In the future there may be a simple mobile app that will turn your phone into basic GPS tracker.

## Additional info

TrackingService will persist positions only if device is registered in database. This, in turn, requires a user account. There are basic API endpoints for user registration, login and keeping user session up.
