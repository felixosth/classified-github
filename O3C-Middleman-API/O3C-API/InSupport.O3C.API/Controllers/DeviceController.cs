using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InSupport.O3C.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace InSupport.O3C.API.Controllers
{
    /// <summary>
    /// The O3CDevice controller.
    /// </summary>
    [ApiController]
    [Route("api", Name = "Device")]
    public class DeviceController : ControllerBase
    {
        private readonly O3CDeviceDbContext _context;

        /// <summary>
        /// The O3CDevice controller constructor.
        /// </summary>
        /// <param name="context"></param>
        public DeviceController(O3CDeviceDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get a device by MAC.
        /// </summary>
        /// <param name="mac">MAC/Serialnumber</param>
        /// <returns></returns>
        [HttpGet("device/{mac}")]
        public async Task<ActionResult<O3CDevice>> GetDevice(string mac)
        {
            var device = await _context.Devices.FindAsync(mac);

            return device != null ?
                device : NotFound();
        }

        /// <summary>
        /// Get all devices.
        /// </summary>
        /// <returns></returns>
        [HttpGet("devices")]
        public async Task<IEnumerable<O3CDevice>> GetAllDevices()
		{
            return await _context.Devices.ToListAsync();
		}
    }
}
