using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using TryggSTORE.Http;

namespace TryggSTORE.Service
{
    public partial class TryggSTOREService : ServiceBase
    {
        internal const string _VERSION = "1.0.3";

        TryggSTOREServer httpServer;

        public TryggSTOREService()
        {
            InitializeComponent();

            #region eventvwr
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
            #endregion

            httpServer = new TryggSTOREServer(ConfigFile.Instance.HttpPort);
            httpServer.OnError += HttpServer_OnError;

            if(ConfigFile.Instance.DebugLog)
                httpServer.OnLog += HttpServer_OnLog;
        }

        private void HttpServer_OnLog(object sender, string e)
        {
            Log(e);
        }

        private void HttpServer_OnError(object sender, Exception e)
        {
            Log(e.ToString(), EventLogEntryType.Error);
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
            httpServer.Start();
        }

        protected override void OnStop()
        {
            httpServer.Stop();
        }

        internal void Debug()
        {
            OnStart(new string[0]);
        }
    }
}
