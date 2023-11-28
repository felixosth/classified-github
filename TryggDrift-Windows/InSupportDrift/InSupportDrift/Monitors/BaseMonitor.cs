using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace InSupport.Drift.Monitor
{
    public abstract class BaseMonitor
    {
        [JsonIgnore]
        public bool Enabled { get; set; } = true;

        public event EventHandler<string> OnLog;

        [JsonIgnore]
        public virtual object Stash { get; }

        [JsonProperty(Order = 1)]
        public abstract string MonitorName { get; }

        public abstract float MonitorVersion { get; }

        public abstract void LoadSettings(Dictionary<string, string> settings);

        protected void Log(string msg) => OnLog?.Invoke(this, msg);

        public virtual void Close()
        {
        }
    }
}
