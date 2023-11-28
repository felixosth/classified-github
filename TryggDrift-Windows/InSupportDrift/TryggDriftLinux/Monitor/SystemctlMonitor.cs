using Microsoft.Extensions.Logging;
using System;

namespace TryggDriftLinux.Monitor
{
    public class SystemctlMonitor : BaseMonitorLinux
    {
        private SystemctlService[] services;
        public SystemctlMonitor(ILogger<TryggDriftWorker> logger, string[] services) : base(logger)
        {
            this.services = new SystemctlService[services.Length];
            for (int i = 0; i < services.Length; i++)
            {
                this.services[i] = new SystemctlService() { Name = services[i], State = "N/A" };
            }
        }

        public override string MonitorName => "Systemctl";

        public override float MonitorVersion => 1f;


        public SystemctlService[] Services
        {
            get
            {
                foreach (var service in services)
                {
                    try
                    {
                        service.State = $"systemctl show {service.Name} --no-page | grep ActiveState".Bash().Split('=')[1].Replace("\n", "");
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex.Message);
                        service.State = "N/A";
                    }
                }

                return services;
            }
        }
    }

    public class SystemctlService
    {
        public string Name { get; set; }
        public string State { get; set; }
    }
}
