# TrackingService
An experimental, work-in-progress GPS tracking service. For web interface, [see this repository](https://github.com/xtul/tracking-web).

## How are new positions registered?

GPS trackers send encoded positions via TCP connections to a user-configured IP:Port. GPS tracker producers/resellers usually provide protocol documentation, which allows anyone to decode these positions to a human-readable format.

TrackingService exposes a different port for each protocol. Since all protocols are different, there has to be a different decoder for each. `Decoder` abstract class makes it easy to make a new decoder. It handles TCP connection and exposes one abstract method to turn encoded message to `Position` object.

After decoding, this `Position` is sent to `PositionCache`. It checks if device is registered in the system (with some help of `DeviceCache`), and if it is, it's stored in memory. Every couple of minutes (or if application shuts down) these positions are persisted in a PostgreSQL database.

Every time a new position is added, `OnPositionRegistered` event is raised. This allows live position updates with SignalR. `PositionsHub` allows authorized users to subscribe to new positions of their devices.

## What is a `device`?

All GPS trackers are identified by a unique code, most commonly their [IMEI](https://en.wikipedia.org/wiki/International_Mobile_Equipment_Identity). The way users can be identified by their ID, username or e-mail, the same devices can be identified by their IMEI or unique code.

TrackingService doesn't care if this device is real or not, it doesn't do any verification. Devices are registered in TrackingService to know which user to contact if new position is registered by the system.

## Sending new positions

Currently the only GPS tracking protocol implemented is `watch`. It's usually used by mobile GPS trackers based on MTK6261 CPU. It is also highly possible that Comarch's wristband uses this protocol - all cheap, chinese wrist devices have common features, battery size (~300mAH), shape, even packaging is usually the same. For example, [this device](https://www.alibaba.com/product-detail/Factory-gps-smartwatch-X6-phone-android_1600210996211.html) is extremely likely to use `watch` protocol.

You can also send position data as JSON - see [Position.cs](https://github.com/xtul/TrackingService/blob/master/TrackingService.Model/Objects/DbSet/Position.cs). By default, JSON decoder listens on port 6001.

In the future there may be a simple mobile app that will turn your phone into basic GPS tracker.
