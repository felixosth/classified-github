using InSupport.Drift.Monitor;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;

namespace InSupport.Drift.Plugins
{
    public class ServiceMonitorManager : BaseMonitor
    {
        public override float MonitorVersion => 2.03f;
        private readonly List<ServiceMonitor> serviceMonitors = new List<ServiceMonitor>();
        internal const string _CFG_KEY = "Service";

        ServiceMonitorConfig config;
        private Dictionary<string, DateTime> lastServiceUptime { get; set; } = new Dictionary<string, DateTime>();

        [JsonProperty(Order = 4)]
        public ServiceMonitor[] ServiceMonitors
        {
            get
            {
                var allServices = ServiceController.GetServices();
                foreach (var serviceMonitor in serviceMonitors)
                {
                    var service = allServices.FirstOrDefault(s => s.ServiceName == serviceMonitor.ServiceName);
                    if (service == null)
                        continue;

                    if (serviceMonitor.CheckService(service) && service.Status == ServiceControllerStatus.Stopped && config.AutoStartStoppedServices)
                    {
                        if (lastServiceUptime.ContainsKey(service.ServiceName))
                        {
                            if (DateTime.Now >= lastServiceUptime[service.ServiceName])
                            {
                                Log($"Trying to start up {service.ServiceName}...");

                                try
                                {
                                    service.Start();
                                    service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                                    Log($"{service.ServiceName} was stopped but it's now up and running.");
                                    lastServiceUptime.Remove(service.ServiceName);
                                }
                                catch (System.ServiceProcess.TimeoutException)
                                {
                                    Log($"{service.ServiceName} is stopped and it failed to start.");
                                }
                                catch (Exception ex)
                                {
                                    Log($"Unable to start the service {service.ServiceName} due to an error: \r\n {ex}");
                                }
                            }
                        }
                        else
                        {
                            var startAfter = DateTime.Now.AddMinutes(config.AutoStartServiceAfterMinutes);
                            Log($"{service.ServiceName} is stopped, we're going to try to restart it some time after {startAfter}.");
                            lastServiceUptime.Add(service.ServiceName, startAfter);
                        }
                    }
                    else if (lastServiceUptime.ContainsKey(service.ServiceName))
                    {
                        Log($"{service.ServiceName} was stopped but it fixed itself somehow!");
                        lastServiceUptime.Remove(service.ServiceName);
                    }
                }

                return serviceMonitors.ToArray();
            }
        }

        public ServiceMonitorManager() : base()
        {
        }

        public override void LoadSettings(Dictionary<string, string> settings)
        {
            if (settings.ContainsKey(_CFG_KEY))
            {
                config = JsonConvert.DeserializeObject<ServiceMonitorConfig>(settings[_CFG_KEY]);

                foreach (string service in config.ServicesToMonitor)
                {
                    serviceMonitors.Add(new ServiceMonitor(service));
                }
            }
            else Enabled = false;
        }

        public override string MonitorName => "Services";
    }

    public class ServiceMonitor
    {
        public string ServiceName { get; set; }

        public string ServiceDisplayName { get; set; }

        private ServiceControllerStatus _status;
        public string Status { get { return _status.ToString(); } }

        public ServiceMonitor(string serviceName)
        {
            this.ServiceName = serviceName;
        }

        public void Reset()
        {
            _status = ServiceControllerStatus.Stopped;
        }

        public bool CheckService(ServiceController service)
        {
            if (service.ServiceName == this.ServiceName)
            {
                ServiceDisplayName = service.DisplayName;
                _status = service.Status;
                return true;
            }
            return false;
        }

        public override string ToString() => ServiceName;
    }
}
