using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InSupport.O3C.API
{
    /// <summary>
    /// The program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Generated.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureLogging((logFactory) => logFactory.AddEventLog())
                .UseWindowsService()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}
