using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using O3C_API.Models;

namespace O3C_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AxisDevicesController : ControllerBase
    {
        private readonly DeviceContext _context;

        public AxisDevicesController(DeviceContext context)
        {
            _context = context;
        }

        // GET: api/AxisDevices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<O3CDevice>>> GetAxisDevices()
        {
            return await _context.O3CDevices.ToListAsync();
        }

        [HttpGet("mac/{mac}")]
        public async Task<ActionResult<O3CDevice>> GetO3CDeviceByMac(string mac)
        {
            var device = await _context.O3CDevices.FirstOrDefaultAsync(device => device.mac == mac);

            return device != null ? device : NotFound();
        }

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<AxisDevice>>> GetAxisDevices()
        //{
        //    return await _context.AxisDevices.ToListAsync();
        //}

        // GET: api/AxisDevices/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AxisDevice>> GetAxisDevice(long id)
        {
            var axisDevice = await _context.AxisDevices.FindAsync(id);

            if (axisDevice == null)
            {
                return NotFound();
            }

            return axisDevice;
        }

        //[HttpGet("something")]
        //public async Task<ActionResult<string>> Something()
        //{
        //    return "hahahah";
        //}

        //[HttpPut("{action=server_up}")]
        //public async Task<IActionResult> ServerUp(string action)
        //{
        //    var o3c = Request.Headers["X-Stserver-Id"].FirstOrDefault();

        //    return NoContent();
        //}
        [HttpPut]
        public async Task<IActionResult> OnNotification(string action, string client_id = null, string client_srcaddr = null)
        {
            var o3c = Request.Headers["X-Stserver-Id"].FirstOrDefault();
            var host = Request.Headers["Host"].FirstOrDefault();

           
            switch(action)
            {
                case "server_up":
                    break;
                case "client_connect":
                    return await ClientConnect(action, client_id, client_srcaddr);
            }

            return NoContent();
        }

        //[HttpPut]
        public async Task<IActionResult> ClientConnect(string action, string client_id, string client_srcaddr)
        {
            var o3c = Request.Headers["X-Stserver-Id"].FirstOrDefault();
            _context.O3CDevices.Add(new O3CDevice()
            {
                client_id = client_id,
                client_srcaddr = client_srcaddr,
                o3c_server = o3c,
                mac = client_id.Split('.')[1]
            }) ;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT: api/AxisDevices/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAxisDevice(long id, AxisDevice axisDevice)
        {
            if (id != axisDevice.Id)
            {
                return BadRequest();
            }

            _context.Entry(axisDevice).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AxisDeviceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/AxisDevices
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AxisDevice>> PostAxisDevice(AxisDevice axisDevice)
        {
            _context.AxisDevices.Add(axisDevice);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAxisDevice), new { id = axisDevice.Id }, axisDevice);
        }

        // DELETE: api/AxisDevices/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAxisDevice(long id)
        {
            var axisDevice = await _context.AxisDevices.FindAsync(id);
            if (axisDevice == null)
            {
                return NotFound();
            }

            _context.AxisDevices.Remove(axisDevice);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AxisDeviceExists(long id)
        {
            return _context.AxisDevices.Any(e => e.Id == id);
        }
    }
}
