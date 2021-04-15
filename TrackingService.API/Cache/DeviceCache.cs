using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using TrackingService.API.Database;
using TrackingService.Model.Objects;
using TrackingService.Model.Objects.DataStructures;
using TrackingService.Model.Objects.DbSet;

namespace TrackingService.API.Cache {
	/// <summary>
	/// Stores positions in-memory for quick access and 
	/// persists them periodically in a database.
	/// </summary>
	public class DeviceCache : DatabaseCache {
		private readonly IServiceScopeFactory _scopeFactory;
		private readonly ILogger<DeviceCache> _logger;
		private readonly Dictionary<string, Device> _devices;
		private readonly Dictionary<int, List<Device>> _userDevices;

		public DeviceCache(ILogger<DeviceCache> logger, IServiceScopeFactory scopeFactory) {
			_scopeFactory = scopeFactory;
			_logger = logger;
			_devices = new();
			_userDevices = new();

			using (var scope = _scopeFactory.CreateScope()) {
				var db = GetContext(scope);
				// convert userDevices to an dictionary
				var users = db.UserDevice.Select(x => x.UserId).ToArray();
				foreach (var user in users) {
					var devicesOfUser = db.UserDevice
						.Where(x => x.UserId == user)
						.Select(x => x.Device)
						.ToList();

					if (!_userDevices.ContainsKey(user)) {
						_userDevices.Add(user, devicesOfUser);
					}
				}

				var devices = db.Devices.ToList();
				foreach (var device in devices) {
					_devices.Add(device.Imei, device);
				}
			}
		}

		public Device GetDevice(int userId, string imei) {
			var deviceExists = DeviceExists(imei, out var foundDevice);
			if (!deviceExists) {
				return null;
			}

			var canSeeDevice = CanUserSeeDevice(userId, foundDevice.Id);

			if (canSeeDevice) {
				return foundDevice;
			} else {
				return null;
			}
		}

		public List<Device> GetDevices(int userId) {
			return _userDevices[userId];
		}

		public async Task CreateDeviceAsync(Device device, int userId) {
			if (_devices.ContainsKey(device.Imei)) {
				return;
			}

			_devices.Add(device.Imei, device);
			_userDevices[userId].Add(device);

			using (var scope = _scopeFactory.CreateScope()) {
				var db = GetContext(scope);

				db.Devices.Add(device);
				db.UserDevice.Add(new UserDevice() {
					UserId = userId,
					Device = device // auto assigns a device ID
				});

				await db.SaveChangesAsync();
			}
		}

		public async Task<bool> ReplaceDeviceAsync(string imei, Device device) {

			// if IMEI was changed...
			if (imei != device.Imei) {
				_devices.Remove(imei);
				_devices.Add(imei, device);
			} else {
				_devices[imei] = device;				
			}		

			using (var scope = _scopeFactory.CreateScope()) {
				var db = GetContext(scope);
				try {
					var entity = db.Devices.FirstOrDefault(x => x.Imei == imei);

					if (entity is not null) {
						entity.CopyValues(device);
						await db.SaveChangesAsync();
					}
				} catch (DbUpdateConcurrencyException) {
					if (!DeviceExists(device.Imei, out _)) {
						return false;
					} else {
						throw;
					}
				}
			}

			return true;
		}

		public bool RemoveDevice(string imei) {
			return _devices.Remove(imei);
		}

		public bool DeviceExists(string imei, out Device device) {
			var searchedDevice = _devices[imei];
			if (searchedDevice is not null) {
				device = searchedDevice;
				return true;
			} else {
				device = null;
				return false;
			}
		}

		public bool CanUserSeeDevice(int userId, Predicate<Device> func) {
			return _userDevices[userId].Exists(func);
		}

		public bool CanUserSeeDevice(int userId, int deviceId) {
			return CanUserSeeDevice(userId, x => x.Id == deviceId);

		}

		public bool CanUserSeeDevice(int userId, string imei) {
			return CanUserSeeDevice(userId, x => x.Imei == imei);
		}
	}
}
