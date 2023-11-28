using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace SarvAI.Milestone.Verifier
{
    public partial class SarvAIService : ServiceBase
    {
        AlarmExporter alarmExporter;

        public static string Server { get; private set; }
        public static string SaveVideoHeader { get; private set; }
        public static string Username { get; private set; }
        public static string Password { get; private set; }

        public static int AlarmPriority { get; private set; }
        public static int StateInProcess { get; private set; }

        public static int PreAndPostTime { get; private set; }

        public SarvAIService()
        {
            InitializeComponent();


            //serviceName = config.AppSettings.Settings["ServiceName"].Value.ToString();

            this.ServiceName = Constants.Service.ServiceName;
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

        protected override void OnStart(string[] args)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            string targetDir = executingAssembly.Location;
            Configuration config = ConfigurationManager.OpenExeConfiguration(targetDir);

            Server = config.AppSettings.Settings["sarvAiServer"].Value;
            SaveVideoHeader = config.AppSettings.Settings["saveVideo"].Value;
            Username = config.AppSettings.Settings["username"].Value;
            Password = config.AppSettings.Settings["password"].Value;

            AlarmPriority = int.Parse(config.AppSettings.Settings["priority"].Value);
            StateInProcess = int.Parse(config.AppSettings.Settings["stateInProcess"].Value);

            PreAndPostTime = int.Parse(config.AppSettings.Settings["preAndPostTime"].Value) * 1000;

            alarmExporter = new AlarmExporter((s) => Log(s), true);
        }

        private void AlarmExporter_OnError(object sender, string e)
        {
            Log(e, EventLogEntryType.Error);
        }

        protected override void OnStop()
        {
            alarmExporter.Close();
        }

        public void DebugStart()
        {
            OnStart(null);
        }

        internal void Log(string msg, EventLogEntryType logType = EventLogEntryType.Information, int eventId = 0)
        {
            try
            {
                this.EventLog.WriteEntry(msg, logType, eventId);
            }
            catch { }
        }
    }
}
