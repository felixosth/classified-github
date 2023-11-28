using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TryggDriftLinux
{
    public class Program
    {
        public static void Main(string[] args)
        {


            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSystemd()
                .ConfigureServices((hostContext, services) =>
                {
                    var configuration = hostContext.Configuration;

                    WorkerOptions options = configuration.GetSection("TryggDrift").Get<WorkerOptions>();

                    services.AddSingleton(options);

                    services.AddHostedService<TryggDriftWorker>();
                });
    }

}
