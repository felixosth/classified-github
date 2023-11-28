using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using InSupport.O3C.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace InSupport.O3C.API.Controllers
{
    /// <summary>
    /// The O3CServer controller.
    /// </summary>
    [ApiController]
    [Route("api")]
    public class ServerController : Controller
    {
        private readonly O3CDeviceDbContext _context;
        private readonly ILogger<DeviceController> logger;
        private string adpUsername, adpPassword, serverList;

        /// <summary>
        /// The server controller constructor.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="config"></param>
        /// <param name="logger"></param>
        public ServerController(O3CDeviceDbContext context, IConfiguration config, ILogger<DeviceController> logger)
        {
            _context = context;
            this.logger = logger;

            serverList = config["O3C:ServerList"];
            adpUsername = config["O3C:Username"];
            adpPassword = config["O3C:Password"];
        }

        /// <summary>
        /// List all O3CServers.
        /// </summary>
        /// <returns></returns>
        [HttpGet("servers")]
        public async Task<ActionResult<IEnumerable<O3CServer>>> GetServers()
        {
            return await _context.Servers.ToListAsync();
        }

        /// <summary>
        /// List all O3CServers from a server cluster.
        /// </summary>
        /// <param name="clusterId"></param>
        /// <returns></returns>
        [HttpGet("servers/{clusterId}")]
        public async Task<ActionResult<IEnumerable<O3CServer>>> GetServersFromCluster(int clusterId)
        {
            return await _context.Servers.Where(server => server.ClusterId == clusterId).ToListAsync();
        }

        /// <summary>
        /// List all devices from a server cluster.
        /// </summary>
        /// <param name="clusterId"></param>
        /// <returns></returns>
        [HttpGet("servers/{clusterId}/devices")]
        public async Task<ActionResult<IEnumerable<O3CDevice>>> GetDevicesFromCluster(int clusterId)
        {
            var serversInCluster = _context.Servers.Where(server => server.ClusterId == clusterId);
            return await _context.Devices.Where(device => serversInCluster.Any(server => server.Name == device.O3CServer)).ToListAsync();
        }

        /// <summary>
        /// Returns all O3C server instances with more than 0 cameras and the count.
        /// </summary>
        /// <returns></returns>
        [HttpGet("servers/count")]
        public async Task<IActionResult> GetServersWithCount()
        {
            return Ok(await _context.Devices.GroupBy(device => device.O3CServer)
                .Select(grp => new { Name = grp.Key, Count = grp.Count() }).ToListAsync());
        }

        /// <summary>
        /// Get a O3CServer by name.
        /// </summary>
        /// <param name="name">stserverid</param>
        /// <returns></returns>
        [HttpGet("server/{name}")]
        public async Task<ActionResult<O3CServer>> GetServer(string name)
        {
            var o3cServer = await _context.Servers.FindAsync(name);

            return o3cServer != null ?
                o3cServer : NotFound();
        }

        /// <summary>
        /// Modify a server, all properties must be mentioned or they will be set to default.
        /// </summary>
        /// <param name="name">The O3CServer name</param>
        /// <param name="server">The whole O3CServer object with all properties included.</param>
        /// <returns></returns>
        [HttpPut("server/{name}")]
        public async Task<IActionResult> PutServer(string name, O3CServer server)
        {
            if (name != server.Name)
            {
                return BadRequest();
            }

            _context.Entry(server).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Servers.Any(s => s.Name == name))
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

        /// <summary>
        /// Set the server hostname/address.
        /// </summary>
        /// <param name="name">O3CServer name</param>
        /// <param name="host">New host</param>
        /// <returns></returns>
        [HttpPut("server/{name}/setHost/{host}")]
        public async Task<IActionResult> SetServerHost(string name, string host)
        {
            var server = await _context.Servers.FindAsync(name);

            if (server != null && server.Host != host)
            {
                server.Host = host;
                _context.Entry(server).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(server);
                //return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Set the server external hostname/address.
        /// </summary>
        /// <param name="name">O3CServer name</param>
        /// <param name="externalHost">New host</param>
        /// <returns></returns>
        [HttpPut("server/{name}/setExternalHost/{externalHost}")]
        public async Task<IActionResult> SetServerExternalHost(string name, string externalHost)
        {
            var server = await _context.Servers.FindAsync(name);

            if (server != null && server.ExternalHost != externalHost)
            {
                server.ExternalHost = (string.IsNullOrWhiteSpace(externalHost) || externalHost.ToLower() == "null" ) ? null : externalHost;
                _context.Entry(server).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(server);
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Set the client port.
        /// </summary>
        /// <param name="name">O3CServer name</param>
        /// <param name="clientPort">New client port</param>
        /// <returns></returns>
        [HttpPut("server/{name}/setClientPort/{clientPort}")]
        public async Task<IActionResult> SetClientPort(string name, int clientPort)
        {
            var server = await _context.Servers.FindAsync(name);

            if (server != null && server.ClientPort != clientPort)
            {
                server.ClientPort = clientPort;
                _context.Entry(server).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(server);
                //return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Set the admin port.
        /// </summary>
        /// <param name="name">O3CServer name</param>
        /// <param name="adminPort">New admin port</param>
        /// <returns></returns>
        [HttpPut("server/{name}/setAdminPort/{adminPort}")]
        public async Task<IActionResult> SetAdminPort(string name, int adminPort)
        {
            var server = await _context.Servers.FindAsync(name);

            if (server != null && server.AdminPort != adminPort)
            {
                server.AdminPort = adminPort;
                _context.Entry(server).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(server);
                //return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Set the server to balance the load between other servers in the same cluster.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        [HttpPut("server/{name}/setLoadBalance/{val}")]
        public async Task<IActionResult> SetLoadBalance(string name, bool val)
        {
            var server = await _context.Servers.FindAsync(name);

            if (server != null && server.ApplyLoadBalancing != val)
            {
                server.ApplyLoadBalancing = val;
                _context.Entry(server).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(server);
                //return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Set the server cluster id.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="clusterId"></param>
        /// <returns></returns>
        [HttpPut("server/{name}/setClusterId/{clusterId}")]
        public async Task<IActionResult> SetClusterId(string name, int clusterId)
        {
            var server = await _context.Servers.FindAsync(name);

            if (server != null && server.ClusterId != clusterId)
            {
                server.ClusterId = clusterId;
                _context.Entry(server).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(server);
                //return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Get all devices within the O3CServer.
        /// </summary>
        /// <param name="name">stserverid</param>
        /// <returns></returns>
        [HttpGet("server/{name}/devices")]
        public async Task<ActionResult<IEnumerable<O3CDevice>>> GetDevicesFromServer(string name)
        {
            return await _context.Devices.Where(d => d.O3CServer == name).ToListAsync();
        }

        /// <summary>
        /// Register a camera via the specified O3C server.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="oak"></param>
        /// <param name="mac"></param>
        /// <returns></returns>
        [HttpGet("server/{name}/register")]
        public async Task<IActionResult> RegisterCamera(string name, string oak, string mac)
        {
            var o3cServer = await _context.Servers.FindAsync(name);
            if (o3cServer == null)
                return NotFound("O3C server not found");

            var uri = new Uri($"http://{o3cServer.Host}:{o3cServer.AdminPort}/admin/dispatch.cgi?action=register" +
                $"&user={adpUsername}" +
                $"&pass={adpPassword}" +
                $"&mac={mac}" +
                $"&oak={oak}" +
                $"&server={serverList}");

            logger.LogInformation("Incoming register command, using url: " + uri.ToString());


            try
            {
                using var httpClient = new HttpClient();
                using var response = await httpClient.GetAsync(uri);

                var responseString = await response.Content.ReadAsStringAsync();

                /* A typical call to the register function looks like this:
                http://127.0.0.1:3128/admin/dispatch.cgi?action=register&user=example&pass=example&mac=00408CABCDEF&oak=1234-5678-90AB&server=example.com:8080,example.com:8081
                */

                return Ok(responseString);
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }
    }
}
