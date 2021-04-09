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
using TrackingService.API.Database;
using TrackingService.Model.Objects;
using TrackingService.Model.Objects.DbSet;

namespace TrackingService.API.Controllers {
	[Route("api/devices")]
	[ApiController]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class DevicesController : ControllerBase {
		private readonly TrackingDbContext _context;
		private const string Bind = "Name,Enabled,LastPositionId,Imei";
		private readonly List<Device> _existsCache;

		public DevicesController(TrackingDbContext context) {
			_context = context;
			_existsCache = new();
		}

		[HttpGet]
		public async Task<IActionResult> GetDevice([FromQuery] string imei) {
			var userId = int.Parse(User.FindFirstValue("Id"));

			var deviceExists = DeviceExists(imei, out var foundDevice);
			if (!deviceExists) {
				return BadRequest();
			}

			var canUserSeeDevice = await _context.UserDevice
				.AnyAsync(x => 
					x.UserId == userId && 
					x.DeviceId == foundDevice.Id
				);
			if (canUserSeeDevice) {
				return Ok(foundDevice);
			}

			return BadRequest();
		}

		[HttpGet]
		[Route("all")]
		public async Task<IActionResult> GetDevices() {
			var userId = int.Parse(User.FindFirstValue("Id"));

			var userDevices = await _context.UserDevice
				.Where(x => x.UserId == userId)
				.Select(x => x.DeviceId)
				.ToListAsync();

			if (userDevices is null || userDevices.Count < 1) {
				return BadRequest();
			}

			var devices = await _context.Devices
				.AsNoTracking()
				.Where(x => userDevices.Contains(x.Id))
				.ToListAsync();

			return Ok(devices);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateDevice([Bind(Bind)] Device device) {
			if (!ModelState.IsValid) {
				return BadRequest(device);
			}

			var deviceExists = DeviceExists(device.Imei, out _);
			if (deviceExists) {
				return BadRequest("Device already exists.");
			}

			_context.Add(device);
			// also create binding to this device for this user
			_context.UserDevice.Add(new() {
				UserId = int.Parse(User.FindFirstValue("Id")),
				DeviceId = device.Id
			});

			await _context.SaveChangesAsync();

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

			try {
				_context.Update(device);
				await _context.SaveChangesAsync();
			} catch (DbUpdateConcurrencyException) {
				if (!DeviceExists(device.Id, out _)) {
					return NotFound();
				} else {
					throw;
				}
			}

			return Ok(device);
		}

		[HttpDelete]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteDevice(int id) {
			var device = await _context.Devices.FindAsync(id);
			_context.Devices.Remove(device);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool DeviceExists(int id, out Device device) {
			var deviceInCache = _existsCache.Where(x => x.Id == id).FirstOrDefault();
			if (deviceInCache is not null) {
				device = deviceInCache;
				return true;
			}

			var searchedDevice = _context.Devices.Find(id);
			if (searchedDevice is not null) {
				device = searchedDevice;
				_existsCache.Add(searchedDevice);
			} else {
				device = null;
			}

			return searchedDevice is not null;
		}

		private bool DeviceExists(string imei, out Device device) {
			var deviceInCache = _existsCache.Where(x => x.Imei == imei).FirstOrDefault();
			if (deviceInCache is not null) {
				device = deviceInCache;
				return true;
			}

			var searchedDevice = _context.Devices.AsNoTracking().Where(x => x.Imei == imei).FirstOrDefault();
			if (searchedDevice is not null) {
				device = searchedDevice;
			} else {
				device = null;
			}

			return searchedDevice is not null;
		}
	}
}
