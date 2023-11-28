using InSupport.O3C.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InSupport.O3C.API.Controllers
{
    /// <summary>
    /// The controller that handles the O3C notifications.
    /// </summary>
    [ApiController]
    [Route("api/notify", Name = "Notify")]
    public class NotifyController : Controller
    {
        private readonly O3CDeviceDbContext _context;
        private readonly ILogger<DeviceController> logger;

        private string adpUsername, adpPassword, serverList;

        /// <summary>
        /// The controller that handles the notify events from the O3C servers.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="config"></param>
        /// <param name="logger"></param>
        public NotifyController(O3CDeviceDbContext context, IConfiguration config, ILogger<DeviceController> logger)
        {
            _context = context;
            this.logger = logger;

            serverList = config["O3C:ServerList"];
            adpUsername = config["O3C:Username"];
            adpPassword = config["O3C:Password"];
        }

        /// <summary>
        /// Used by the O3C notify, ClientHello. Load balancing is applied here.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="client_id"></param>
        /// <returns></returns>
        [HttpHead]
        public async Task<IActionResult> ClientHello(string action, string client_id)
        {
            var o3cServerName = Request.Headers["X-Stserver-Id"].FirstOrDefault();
            var o3cServer = await _context.Servers.FindAsync(o3cServerName);
            if (o3cServer == null)
                throw new Exception("O3C server is null");

            var mac = client_id.Split('.')[1];
            logger.LogInformation(mac + " says hello to " + o3cServer.Name);

            if (o3cServer.ApplyLoadBalancing)
            {
                var serversWithDeviceCount = _context.Servers
                    .Where(server => server.IsUp && server.ApplyLoadBalancing && server.ClusterId == o3cServer.ClusterId)
                    .Select(server => new
                    {
                        server.Name,
                        server.Host,
                        server.ClientPort,
                        server.AdminPort,
                        server.ExternalHost,
                        Devices = _context.Devices.Where(device => device.O3CServer == server.Name).Count()
                    })
                    .OrderBy(server => server.Devices);

                var myDeviceCount = _context.Devices.Where(device => device.O3CServer == o3cServer.Name).Count();

                var serverWithLeastDeviceCount = serversWithDeviceCount.FirstOrDefault();

                if (serverWithLeastDeviceCount != null &&
                    o3cServer.Name != serverWithLeastDeviceCount.Name &&
                    myDeviceCount > serverWithLeastDeviceCount.Devices)
                {
                    logger.LogInformation(mac + " is redirected to " + serverWithLeastDeviceCount.Name);
                    return Redirect($"{serverWithLeastDeviceCount.ExternalHost ?? serverWithLeastDeviceCount.Host}:{serverWithLeastDeviceCount.ClientPort}");
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Used by the O3C notify, ServerUp and ClientConnect.
        /// </summary>
        /// <param name="action">server_up or client_connect</param>
        /// <param name="client_id"></param>
        /// <param name="client_srcaddr"></param>
        /// <param name="product"></param>
        /// <param name="firmwareVersion"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> OnPutNotification(string action, string client_id = null, string client_srcaddr = null, string product = null, string firmwareVersion = null)
        {
            var o3c = Request.Headers["X-Stserver-Id"].FirstOrDefault();
            var host = Request.Headers["Host"].FirstOrDefault();

            switch (action)
            {
                case "server_up":
                    return await ServerUp(o3c, host);
                case "client_connect":
                    return await ClientConnect(o3c, client_id, client_srcaddr, product, firmwareVersion);
                default:
                    return BadRequest();
            }
        }

        private async Task<IActionResult> ServerUp(string o3cServerName, string host)
        {
            var o3cServer = await _context.Servers.FirstOrDefaultAsync(server => server.Name == o3cServerName);

            if (o3cServer == null)
            {
                o3cServer = new O3CServer()
                {
                    AdminPort = 3128,
                    ClientPort = 8080,
                    Host = host,
                    Name = o3cServerName,
                    IsUp = true
                };
                _context.Servers.Add(o3cServer);
                await _context.SaveChangesAsync();
            }
            else if (o3cServer.IsUp == false)
            {
                _context.Entry(o3cServer).State = EntityState.Modified;
                o3cServer.IsUp = true;
                await _context.SaveChangesAsync();
            }

            logger.LogInformation(o3cServer.Name + " is up");

            return Ok();
        }

        private async Task<IActionResult> ClientConnect(string o3cServerName, string client_id, string client_srcaddr, string product, string firmwareVersion)
        {
            var o3cServer = await _context.Servers.FindAsync(o3cServerName);
            var mac = client_id.Split('.')[1];

            if (o3cServer == null)
                throw new Exception("O3C server is null");

            var device = await _context.Devices.FindAsync(mac);
            if (device == null)
            {
                _context.Devices.Add(new O3CDevice()
                {
                    O3CServer = o3cServer.Name,
                    MAC = mac,
                    IsUp = true,
                    Product = product,
                    Firmware = firmwareVersion
                });

                await _context.SaveChangesAsync();
            }
            else if (device.IsUp == false || device.O3CServer != o3cServer.Name)
            {
                device.IsUp = true;
                device.O3CServer = o3cServer.Name;
                _context.Entry(device).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }

            logger.LogInformation(mac + " connected to " + o3cServer.Name);


            return Ok();
        }

        /// <summary>
        /// Used by the O3C notify, ServerDown and ClientDisconnect.
        /// </summary>
        /// <param name="action">server_down or client_disconnect</param>
        /// <param name="client_id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> OnDeleteNotification(string action, string client_id = null)
        {
            var o3c = Request.Headers["X-Stserver-Id"].FirstOrDefault();
            var host = Request.Headers["Host"].FirstOrDefault();

            switch (action)
            {
                case "server_down":
                    return await ServerDown(o3c, host);
                case "client_disconnect":
                    return await ClientDisconnect(o3c, client_id);
                default:
                    return BadRequest();
            }
        }

        private async Task<IActionResult> ServerDown(string o3cServerName, string host)
        {
            var o3cServer = await _context.Servers.FindAsync(o3cServerName);

            if (o3cServer == null)
                return NotFound();
            else
            {
                o3cServer.IsUp = false;
                _context.Entry(o3cServer).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                foreach (var device in _context.Devices.Where(device => device.O3CServer == o3cServer.Name))
                {
                    device.IsUp = false;
                    _context.Entry(device).State = EntityState.Modified;
                }

                await _context.SaveChangesAsync();

                logger.LogInformation(o3cServer.Name + " is down");

                return Ok();
            }
        }


        private async Task<IActionResult> ClientDisconnect(string o3cServerName, string client_id)
        {
            var o3cServer = await _context.Servers.FindAsync(o3cServerName);

            if (o3cServer == null)
            {
                throw new Exception("O3C server is null");
            }

            var mac = client_id.Split('.')[1];
            var device = await _context.Devices.FindAsync(mac);
            if (device != null)
            {
                device.IsUp = false;
                _context.Entry(device).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }

            logger.LogInformation(mac + " disconnected");


            return Ok();
        }
    }
}
