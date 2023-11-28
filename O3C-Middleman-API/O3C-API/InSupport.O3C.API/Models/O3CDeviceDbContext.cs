using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InSupport.O3C.API.Models
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public class O3CDeviceDbContext : DbContext
    {
        public DbSet<O3CDevice> Devices { get; set; }
        public DbSet<O3CServer> Servers { get; set; }

        public O3CDeviceDbContext(DbContextOptions<O3CDeviceDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<O3CServer>()
                .HasIndex(o => o.Name)
                .IsUnique();

            modelBuilder.Entity<O3CDevice>()
                .HasIndex(d => d.MAC)
                .IsUnique();
            modelBuilder.Entity<O3CDevice>()
                .HasIndex(d => d.O3CServer);
        }
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
