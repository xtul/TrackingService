using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrackingService.API.Cache;
using TrackingService.API.Database;
using TrackingService.Model.Objects;
using TrackingService.Model.Objects.DbSet;

namespace TrackingService.API.Controllers {
	[Route("api/devices")]
	[ApiController]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class DevicesController : ControllerBase {
		private const string Bind = "Name,Enabled,Imei";
		private readonly DeviceCache _deviceCache;

		public DevicesController(DeviceCache cache) {
			_deviceCache = cache;
		}

		[HttpGet]
		[Route("{imei}")]
		public IActionResult GetDevice(string imei) {
			var userId = int.Parse(User.FindFirstValue("Id"));

			var deviceExists = _deviceCache.DeviceExists(imei, out var foundDevice);
			if (!deviceExists) {
				return BadRequest();
			}

			var canUserSeeDevice = _deviceCache.CanUserSeeDevice(userId, foundDevice.Id);
			if (canUserSeeDevice) {
				return Ok(foundDevice);
			}

			return BadRequest();
		}

		[HttpGet]
		public IActionResult GetDevices() {
			var userId = int.Parse(User.FindFirstValue("Id"));

			var userDevices = _deviceCache.GetDevices(userId);

			if (userDevices is null || userDevices.Count < 1) {
				return BadRequest();
			}

			return Ok(userDevices);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateDevice([Bind(Bind)][FromBody] Device device) {
			if (!ModelState.IsValid) {
				return BadRequest(device);
			}


			var deviceExists = _deviceCache.DeviceExists(device.Imei, out _);
			if (deviceExists) {
				return BadRequest("Device already exists.");
			}

			var userId = int.Parse(User.FindFirstValue("Id"));
			await _deviceCache.CreateDeviceAsync(device, userId);

			return Ok(device);
		}

		[HttpPut]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> UpdateDevice(int id, [Bind(Bind)] Device device) {
			if (id != device.Id) {
				return BadRequest();
			}

			if (!ModelState.IsValid) {
				return BadRequest();
			}

			await _deviceCache.ReplaceDeviceAsync(device.Imei, device);

			return Ok(device);
		}

		[HttpDelete]
		[ValidateAntiForgeryToken]
		public IActionResult DeleteDevice(string imei) {
			var removed = _deviceCache.RemoveDevice(imei);

			if (removed) {
				return Ok("Deleted.");
			}

			return BadRequest();
		}
	}
}
