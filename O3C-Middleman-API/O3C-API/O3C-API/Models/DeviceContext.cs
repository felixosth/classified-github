using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace O3C_API.Models
{
    public class DeviceContext : DbContext
    {
        public DbSet<AxisDevice> AxisDevices { get; set; }
        public DbSet<O3CDevice> O3CDevices { get; set; }
        public DbSet<O3CServer> O3CServers { get; set; }

        public DeviceContext(DbContextOptions<DeviceContext> options) : base(options)
        {
        }
    }
}
