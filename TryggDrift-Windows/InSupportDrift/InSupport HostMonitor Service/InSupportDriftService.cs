using InSupport;
using InSupport.Drift;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceProcess;

namespace InSupport_HostMonitor_Service
{
    public partial class InSupportDriftService : ServiceBase
    {
        HostMonitor hostMonitor;
        private int InstanceId { get; set; }
        public bool Running = false;

        public InSupportDriftService(int instanceId = -1)
        {
            InitializeComponent();
            this.InstanceId = instanceId;

            this.ServiceName = "TryggDrift" + (InstanceId > -1 ? "-" + InstanceId : "");
            this.EventLog.Source = this.ServiceName;
            this.EventLog.Log = "Application";

            try
            {
                ((ISupportInitialize)(this.EventLog)).BeginInit();
                if (!EventLog.SourceExists(this.EventLog.Source))
                {
                    EventLog.CreateEventSource(this.EventLog.Source, this.EventLog.Log);
                }
            ((ISupportInitialize)(this.EventLog)).EndInit();
            }
            catch
            {
            }
        }

        public void DebugStart(string[] args)
        {
            OnStart(args);
        }

        public void Log(string msg, EventLogEntryType logType = EventLogEntryType.Information)
        {
            try
            {
                this.EventLog.WriteEntry(msg, logType);
            }
            catch { }
        }

        protected override void OnStart(string[] args)
        {
            Running = true;
            hostMonitor = new HostMonitor()
            {
                InstanceId = InstanceId
            };
            hostMonitor.ConfigChanged += HostMonitor_ConfigChanged;

            hostMonitor.OnError += (s, error) =>
            {
                Log(error, EventLogEntryType.Warning);
            };
            hostMonitor.OnLog += (s, msg) =>
            {
                Log(msg, EventLogEntryType.Information);
            };
            hostMonitor.OnReport += HostMonitor_OnReport;
            var settings = Cfg.Parse(Cfg.GetPath(InstanceId));

            hostMonitor.Start(settings);
            //Log("Initialized with settings: " + string.Join(",", settings));
            Log("Initialized with plugins: " + string.Join(", ", hostMonitor.PluginNames ?? new string[] { "No plugins" }));
        }

        private void HostMonitor_ConfigChanged(object sender, EventArgs e)
        {
            hostMonitor.Stop();
            OnStart(null);
        }

        private void HostMonitor_OnReport(object sender, string e)
        {
            Log("Report result: " + e);
        }

        protected override void OnStop()
        {
            Running = false;
            hostMonitor.Stop();
        }
    }
}
