using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;

namespace TryggDriftLinux.Monitor
{
    public abstract class BaseMonitorLinux
    {
        [JsonIgnore]
        public bool Enabled { get; set; } = true;

        public event EventHandler<string> OnLog;

        [JsonIgnore]
        public virtual object Stash { get; }

        protected ILogger<TryggDriftWorker> Logger { get; private set; }

        [JsonProperty(Order = 1)]
        public abstract string MonitorName { get; }

        public abstract float MonitorVersion { get; }

        // Constructor
        public BaseMonitorLinux(ILogger<TryggDriftWorker> logger) => Logger = logger;

        protected void Log(string msg) => OnLog?.Invoke(this, msg);

        public virtual void Close() { }
    }
}
