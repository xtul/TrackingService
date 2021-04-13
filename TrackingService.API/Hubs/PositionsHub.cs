using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TrackingService.API.Cache;

namespace TrackingService.API.Hubs {
	[Authorize]
	public class PositionsHub : Hub {
		private readonly List<string> _subscribedDevices = new();
		private readonly PositionCache _positionCache;
		private readonly DeviceCache _deviceCache;
		private IClientProxy _caller;

		public PositionsHub(PositionCache positionCache, DeviceCache deviceCache) {
			_positionCache = positionCache;
			_deviceCache = deviceCache;
		}

		private async void OnPositionRegistered(object sender, PositionAddedEventArgs e) {
			if (!_subscribedDevices.Contains(e.Imei)) {
				return;
			}

			var json = JsonConvert.SerializeObject(e.Position);
			await _caller.SendAsync("position", e.Imei, json);
		}

		public async Task SubscribeToDevices(string[] deviceImeis) {
			if (deviceImeis is null || deviceImeis.Length < 1) {
				await Clients.Caller.SendAsync("noImeis");
				return;
			}

			Console.WriteLine($"Received connection from {Context.ConnectionId}, requesting following devices:");
			foreach(var imei in deviceImeis) {
				Console.WriteLine($"    {imei}");
			}

			var user = Context.User;

			if (user is null || !user.Identity.IsAuthenticated) {
				await Clients.Caller.SendAsync("unauthorized");
				return;
			}

			var userId = int.Parse(user.FindFirstValue("Id"));

			// check if user has access to all devices, refuse if not authorized to any
			foreach (var imei in deviceImeis) {
				var canSeeDevice = _deviceCache.CanUserSeeDevice(userId, imei);
				if (!canSeeDevice) {
					await Clients.Caller.SendAsync("badImeis");
					return;
				}
			}

			// send first positions and add to subscribed devices
			foreach (var imei in deviceImeis) {
				var position = _positionCache.GetNewestByImei(imei);

				// device may not have first position
				if (position is null) {
					continue;
				}

				var json = JsonConvert.SerializeObject(position);
				await Clients.Caller.SendAsync("position", imei, json);

				_subscribedDevices.Add(imei);
				_caller = Clients.Caller;
			}

			// if everything is OK, begin subscription
			_positionCache.OnPositionRegistered += OnPositionRegistered;
		}
	}
}
