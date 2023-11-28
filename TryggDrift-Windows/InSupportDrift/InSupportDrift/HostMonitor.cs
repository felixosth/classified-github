using InSupport.Drift.Monitor;
using InSupport.Drift.Plugins;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InSupport.Drift
{
    public class HostMonitor
    {
        const double _VERSION = 1.2;

        public string[] PluginNames { get; private set; }

        public static bool LogMode { get; set; }

        public event EventHandler<string> OnError;

        public event EventHandler<string> OnReport;
        public event EventHandler<string> OnLog;
        public event EventHandler ConfigChanged;
        private int Interval { get; set; }

        private string ApiUrl { get; set; }

        private readonly List<BaseMonitor> Monitors;
        private readonly List<CustomPlugin> Plugins;

        private bool Running { get; set; }

        private Dictionary<string, string> Settings;

        private string AddCode;

        private bool IgnoreSSL { get; set; }

        private string rawCfg;
        public int InstanceId { get; set; } = -1;

        public const string MonitorSerializationTimeoutSettingsKey = "MonitorSerializationTimeout";
        private int MonitorSerializationTimeoutSeconds = 300;

        public HostMonitor()
        {
            Running = false;
            Interval = 600; // default 10 mins
            Monitors = new List<BaseMonitor>();
            Plugins = new List<CustomPlugin>();
        }

        public void TriggerOnError(string error)
        {
            OnError?.Invoke(this, error);
        }

        public void Start(Dictionary<string, string> settings)
        {
            rawCfg = File.ReadAllText(Cfg.GetPath(InstanceId));
            this.Settings = settings;

            if (settings.ContainsKey("DriftUrl"))
            {
                ApiUrl = settings["DriftUrl"] + "/backend/api.php";
            }

            if (settings.ContainsKey("LogMode") && settings["LogMode"].ToLower() == "true")
                LogMode = true;
            else
                LogMode = false;

            if (settings.ContainsKey("IgnoreSSL") && settings["IgnoreSSL"].ToLower() == "true")
                IgnoreSSL = true;
            else
                IgnoreSSL = false;

            if (IgnoreSSL)
                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            AddCode = settings.ContainsKey("AddCode") ? settings["AddCode"] : "No addcode";

            int tmpInterval = Interval;
            if (settings.ContainsKey("Interval") && int.TryParse(settings["Interval"], out tmpInterval))
            {
                Interval = tmpInterval;
            }

            if (settings.ContainsKey(MonitorSerializationTimeoutSettingsKey) && int.TryParse(settings[MonitorSerializationTimeoutSettingsKey], out int tmpMonitorSerializationTimeout))
            {
                MonitorSerializationTimeoutSeconds = tmpMonitorSerializationTimeout;
            }

            AddPlugins();

            if (settings.ContainsKey("DisablePlugins"))
            {
                var plugins = settings["DisablePlugins"].Split(',');
                foreach (var plugin in plugins)
                {
                    foreach (var monitor in Monitors)
                    {
                        if (monitor.MonitorName == plugin)
                            monitor.Enabled = false;
                    }
                }
            }

            foreach (var monitor in Monitors.Where(m => m.Enabled))
            {
                monitor.LoadSettings(Settings);
            }

            Running = true;
            new Thread(LoopThread) { IsBackground = true }.Start();
        }


        private void AddPlugins()
        {
            var dir = AppDomain.CurrentDomain.BaseDirectory + "plugins";
            if (Directory.Exists(dir))
            {
                var files = Directory.GetFiles(dir, "*.dll", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    try
                    {
                        Assembly assembly = Assembly.LoadFrom(file);

                        foreach (var type in assembly.DefinedTypes)
                        {
                            if (type.IsSubclassOf(typeof(BaseMonitor)))
                            {
                                if (Activator.CreateInstance(type) is BaseMonitor plugin)
                                {
                                    plugin.OnLog += Plugin_OnLog;
                                    Monitors.Add(plugin);
                                }
                            }
                            else if (type.IsSubclassOf(typeof(CustomPlugin)))
                            {
                                if (Activator.CreateInstance(type, new object[] { Settings, this }) is CustomPlugin plugin)
                                    Plugins.Add(plugin);
                            }
                        }
                    }
                    catch/*(Exception ex)*/
                    {
                        //if(LogMode)
                        //{
                        //    Log(ex.Message + "\r\n" + ex.ToString());
                        //}
                    }
                }

                PluginNames = new string[Monitors.Count];
                for (int i = 0; i < Monitors.Count; i++)
                {
                    PluginNames[i] = Monitors[i].MonitorName + " v" + Monitors[i].MonitorVersion;
                }

            }
        }

        private void Plugin_OnLog(object sender, string e)
        {
            OnLog?.Invoke(sender, (sender as BaseMonitor).MonitorName + ": " + e);
        }

        private void LoopThread()
        {
            var mguid = GetMGUID();

            if (InstanceId > -1)
                mguid += "-" + InstanceId;

            while (Running)
            {
                try
                {
                    var data = new
                    {
                        action = "reportIn",
                        mguid,
                        addCode = AddCode,
                        //monitors = JsonConvert.SerializeObject(EnabledMonitors),
                        monitors = SerializeEnabledMonitors(),
                        plugins = Plugins,
                        config = rawCfg,
                        version = _VERSION,
                        name = Settings["Name"],
                        stash = GetStashes()
                    };

                    var payload = JsonConvert.SerializeObject(data);

                    //if (Debugger.IsAttached)
                    //    Debugger.Break();

                    if (LogMode)
                        OnReport?.Invoke(this, payload);

                    string result = SendData(ApiUrl, payload);

                    OnReport?.Invoke(this, result);

                    JToken r = JsonConvert.DeserializeObject<JToken>(result);

                    if ((bool)r["success"] == true)
                    {
                        var newConfig = r.Value<string>("newConfig");

                        if (newConfig != null)
                        {
                            new Thread(() => UpdateConfigFile(newConfig)) { IsBackground = true }.Start();
                        }
                    }
                    else if ((bool)r["success"] == false)
                    {
                        throw new Exception("Unsuccessful report");
                    }
                }
                catch (Exception ex)
                {
                    OnError?.Invoke(this, ex.Message + "\r\n\r\n" + ex.ToString());
                }

                var nextCheck = DateTime.Now.AddSeconds(Interval);
                while (nextCheck >= DateTime.Now && Running)
                {
                    Thread.Sleep(1000);
                }
            }
        }

        /// <summary>
        /// Serialize monitors and cancel the process if it times out
        /// </summary>
        /// <returns>Non timeout enabled monitors</returns>
        private string SerializeEnabledMonitors()
        {
            string serialized = "[";

            var enabledMonitors = EnabledMonitors;

            for (int i = 0; i < enabledMonitors.Length; i++)
            {
                var serializeTask = Task.Run(() =>
                {
                    return JsonConvert.SerializeObject(enabledMonitors[i]);
                });

                if (serializeTask.Wait(TimeSpan.FromSeconds(MonitorSerializationTimeoutSeconds)))
                {
                    serialized += serializeTask.Result + (i + 1 != enabledMonitors.Length ? "," : "");
                }
                else
                {
                    throw new Exception($"Serialization failed for {enabledMonitors[i].MonitorName}. Reason: timeout at {MonitorSerializationTimeoutSeconds} seconds");
                    //OnError?.Invoke(this, $"Serialization failed for {enabledMonitors[i].MonitorName}. Reason: timeout at {MonitorSerializationTimeoutSeconds} seconds");
                }
            }

            serialized += "]";

            return serialized;
        }

        BaseMonitor[] EnabledMonitors
        {
            get
            {
                var enabledMonitors = new List<BaseMonitor>();
                foreach (var monitor in Monitors)
                {
                    if (monitor.Enabled)
                        enabledMonitors.Add(monitor);
                }

                return enabledMonitors.ToArray();
            }
        }

        void UpdateConfigFile(string content)
        {
            var cfgPath = Cfg.GetPath();

            File.WriteAllText(cfgPath, content);
            ConfigChanged?.Invoke(this, new EventArgs());
        }

        string GetStashes()
        {
            var monitors = EnabledMonitors;
            var stashes = new Dictionary<string, object>();

            foreach (var monitor in monitors)
            {
                if (monitor.Stash != null)
                    stashes.Add(monitor.MonitorName, monitor.Stash);
            }
            if (stashes.Count == 0)
                return null;

            return JsonConvert.SerializeObject(stashes);
        }


        private static string cachedMguid = null;
        public static string GetMGUID()
        {
            if (cachedMguid != null)
                return cachedMguid;

            string x64Result = string.Empty;
            string x86Result = string.Empty;
            RegistryKey keyBaseX64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            RegistryKey keyBaseX86 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            RegistryKey keyX64 = keyBaseX64.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography", RegistryKeyPermissionCheck.ReadSubTree);
            RegistryKey keyX86 = keyBaseX86.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography", RegistryKeyPermissionCheck.ReadSubTree);
            object resultObjX64 = keyX64.GetValue("MachineGuid", (object)"default");
            object resultObjX86 = keyX86.GetValue("MachineGuid", (object)"default");
            keyX64.Close();
            keyX86.Close();
            keyBaseX64.Close();
            keyBaseX86.Close();
            keyX64.Dispose();
            keyX86.Dispose();
            keyBaseX64.Dispose();
            keyBaseX86.Dispose();
            keyX64 = null;
            keyX86 = null;
            keyBaseX64 = null;
            keyBaseX86 = null;
            if (resultObjX64 != null && resultObjX64.ToString() != "default")
            {
                cachedMguid = resultObjX64.ToString();
                return resultObjX64.ToString();
            }
            if (resultObjX86 != null && resultObjX86.ToString() != "default")
            {
                cachedMguid = resultObjX86.ToString();
                return resultObjX86.ToString();
            }

            return "not found";
        }

        private string SendData(string uri, string payload)
        {
            using (var client = new HttpClient())
            {


                var content = new StringContent(payload, Encoding.UTF8, "application/json");
                var result = client.PostAsync(uri, content).Result.Content.ReadAsStringAsync().Result;
                return result;
            }
        }

        public void Stop()
        {
            Running = false;
            //while (loopThreadIsAlive) { Thread.Sleep(50); }
        }

        public static void Log(string text)
        {
            string logfile = AppDomain.CurrentDomain.BaseDirectory + "log.txt";
            string message = string.Format("[{0} {1}] {2}", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString(), text);
            File.AppendAllText(logfile, message + "\r\n");
        }
    }
}
