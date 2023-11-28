using AxisPeopleCounterPlugin;
using InSupport.Drift.Monitor;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

namespace InSupport.Drift.Plugins
{
    public class PeopleCounterMonitor : BaseMonitor
    {
        [JsonIgnore]
        internal const string _EncryptKey = "f#a48kofK485Asofasof!kas%o651Kfoaskfo%ASFjI#&aD21";

        public override float MonitorVersion => 0.1f;
        public override string MonitorName => "PeopleCounterMonitor";
        DateTime lastCheck;

        [JsonIgnore]
        internal const string _settingsKey = "PeopleCounters";

        private PeopleCounter[] _peopleCounters;

        [JsonIgnore]
        public int PeopleCounterCount => _peopleCounters.Length;

        public PeopleCounterMonitor() : base()
        {
            lastCheck = DateTime.Now.AddDays(-1);

        }

        public override void LoadSettings(Dictionary<string, string> settings)
        {
            if (settings.ContainsKey(_settingsKey))
            {
                _peopleCounters = JsonConvert.DeserializeObject<PeopleCounter[]>(settings[_settingsKey]);

                foreach (var counter in _peopleCounters)
                {
                    counter.OnLog += Counter_OnLog;
                }
            }
        }

        private void Counter_OnLog(object sender, string e)
        {
            Log((sender as PeopleCounter).IP + ": " + e);
        }

        [JsonIgnore]
        public override object Stash
        {
            get
            {
                var parametersList = new Dictionary<string, object>();

                foreach (var counter in _peopleCounters)
                {
                    parametersList.Add(counter.IP, new
                    {
                        counter.Parameters,
                        Name = counter._name
                    });
                }
                return parametersList;
            }
        }

        public PeopleCounter[] PeopleCounters
        {
            get
            {
                new MassUpdater(_peopleCounters, lastCheck).Update();
                lastCheck = DateTime.Now;

                return _peopleCounters;
            }
        }
    }

    internal class MassUpdater
    {
        private List<PeopleCounter> peopleCounters;
        private bool[] completedCounters;

        DateTime lastCheck;

        public MassUpdater(IEnumerable<PeopleCounter> peopleCounters, DateTime lastCheck)
        {
            this.lastCheck = lastCheck;
            this.peopleCounters = peopleCounters.ToList();
            completedCounters = new bool[peopleCounters.Count()];
        }

        public void Update()
        {
            foreach (var counter in peopleCounters)
            {
                new Thread(() => SingleUpdate(counter)).Start();
            }

            DateTime timeout = DateTime.Now.AddMinutes(15);

            while (completedCounters.Where(b => b == true).Count() != completedCounters.Length && DateTime.Now < timeout)
            {
            }

        }

        private void SingleUpdate(PeopleCounter counter)
        {
            counter.Update();

            if (lastCheck.Day != DateTime.Now.Day)
            {
                if (counter.Error == null)
                    counter.ComparePreviousWeek();
            }

            completedCounters[peopleCounters.IndexOf(counter)] = true;
        }
    }

    public class PeopleCounter
    {
        public string IP { get; set; }
        //public bool IsDown { get; set; }

        public event EventHandler<string> OnLog;

        [JsonIgnore]
        public string _name;
        public string Name { set { _name = value; } }

        public int Status => (int)_status;
        private PeopleCounterStatus _status { get; set; } = PeopleCounterStatus.Up;

        private DateTime lastUp = DateTime.Now;

        private string _username;
        public string Username { set { _username = value; } }

        public bool Obsolete { get; set; }

        private string _encryptedPassword;
        public string EncryptedPassword { set { _encryptedPassword = value; } }
        private string DecryptedPassword => StringCipher.Decrypt(_encryptedPassword, PeopleCounterMonitor._EncryptKey);

        private const string _getParamsUrl = "/local/people-counter/.api?params.json";
        private const string _getDaysListUrl = "/local/people-counter/.api?list-cnt.json";
        private const string _getLogsUrl = "/local/people-counter/.api?show-logs"; // Only works for TrueView
        private const string _getLiveDataUrl = "/local/people-counter/.api?live-sum.json";
        private const string _nativeFirmwareParamUrl = "/axis-cgi/param.cgi?action=list&group=Properties.Firmware";

        private const string _getOldLiveDataUrl = "/local/people-counter/live_sum.js";
        private string GetExportLink(DateTime dateTime) => $"/local/people-counter/.api?export-xml&date={dateTime.ToString("yyyyMMdd")}&res=24h";

        public string Error { get; set; }

        [JsonIgnore]
        public PeopleCounterParameters Parameters { get; private set; }

        [JsonIgnore]
        public PeopleCounterLiveData LiveData { get; set; }

        [JsonIgnore]
        public float In_DifferenceSinceLastWeek { get; set; } //% percentage
        [JsonIgnore]
        public float Out_DifferenceSinceLastWeek { get; set; } //% percentage

        public void Update()
        {
            Error = null;
            Dictionary<string, string> nativeParams = null;

            try
            {
                if (Obsolete)
                {
                    LiveData = Refresh<PeopleCounterLiveData>(_getOldLiveDataUrl);
                    _status = PeopleCounterStatus.Up;
                    lastUp = DateTime.Now;
                    return;
                }

                try
                {
                    using (var wc = GetWebClient())
                    {
                        nativeParams = wc.DownloadString($"http://{IP}{_nativeFirmwareParamUrl}").Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).Dictionary('=');
                    }
                }
                catch (Exception ex)
                {
                    OnLog?.Invoke(this, "Failed to get native params\r\n" + ex.Message);
                }

                if (Parameters == null)
                {

                    Parameters = Refresh<PeopleCounterParameters>(_getParamsUrl);
                    Parameters.Native = nativeParams;
                }

                Out_DifferenceSinceLastWeek = default;
                In_DifferenceSinceLastWeek = default;

                LiveData = Refresh<PeopleCounterLiveData>(_getLiveDataUrl);


                _status = PeopleCounterStatus.Up;
                lastUp = DateTime.Now;
            }
            catch (Exception ex)
            {
                if (ex is WebException)
                {
                    var response = (ex as WebException).Response as HttpWebResponse;
                    if (response != null)
                    {
                        Error = ((int)response.StatusCode).ToString();
                    }
                }
                else
                    Error = ex.Message;

                Parameters = null;
                LiveData = null;

                if (nativeParams != null)
                    _status = PeopleCounterStatus.AppNotRunning;
                else
                    _status = lastUp.AddHours(4) > DateTime.Now ? PeopleCounterStatus.NotResponding : PeopleCounterStatus.Down;
            }
        }

        public void ComparePreviousWeek()
        {
            var previousAddress = $"http://{IP}{GetExportLink(DateTime.Now.AddDays(-8))}";
            var nowAddress = $"http://{IP}{GetExportLink(DateTime.Now.AddDays(-1))}";

            using (var wc = GetWebClient())
            {
                try
                {
                    var previousData = CountData.Deserialize(wc.DownloadString(previousAddress)).ToPretty();
                    var nowData = CountData.Deserialize(wc.DownloadString(nowAddress)).ToPretty();

                    if (previousData.In != 0 && nowData.In != 0)
                    {
                        In_DifferenceSinceLastWeek = PercentageDifference(previousData.In, nowData.In);
                    }
                    if (previousData.Out != 0 && nowData.Out != 0)
                    {
                        Out_DifferenceSinceLastWeek = PercentageDifference(previousData.Out, nowData.Out);
                    }
                }
                catch { }
            }
        }

        float PercentageDifference(float newValue, float originalValue) => ((newValue - originalValue) / originalValue) * 100f;


        private T Refresh<T>(string url)
        {
            using (TimedWebClient wc = GetWebClient())
            {
                string data = wc.DownloadString(new Uri($"http://{IP}{url}"));

                return JsonConvert.DeserializeObject<T>(data);
            }
        }

        [Obsolete]
        private List<string> GetCriticalLogs(int limit)
        {
            string logData = null;
            List<string> criticalLogs = new List<string>();

            using (TimedWebClient wc = GetWebClient())
            {
                logData = wc.DownloadString(new Uri($"http://{IP}{_getLogsUrl}"));
            }

            if (logData != null)
            {
                string[] logEntries = logData.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var logEntry in logEntries.Reverse())
                {
                    var match = new Regex("(?<=<)(.*?)(?=>)").Match(logEntry);
                    if (match.Value == "CRITICAL")
                    {
                        criticalLogs.Add(logEntry);
                    }
                    if (criticalLogs.Count >= limit)
                        break;
                }
            }
            return criticalLogs;
        }

        TimedWebClient GetWebClient() => new TimedWebClient() { Timeout = 10000, Credentials = new NetworkCredential(_username, DecryptedPassword) };
    }

    public enum PeopleCounterStatus
    {
        Up = 1,
        NotResponding = 2,
        AppNotRunning = 3,
        Down = 4
    }

    public class PeopleCounterList
    {
        [JsonProperty("timestamp")]
        public string TimeStamp { get; set; }


        [JsonProperty("days")]
        public string[] Days { get; set; }
    }


    public class PeopleCounterParameters
    {
        public CounterParams Counter { get; set; }
        public BuildParams Build { get; set; }
        public StaticParams Static { get; set; }

        public Dictionary<string, string> Native { get; set; }

        public class CounterParams
        {
            public int Enabled { get; set; }
            public string Name { get; set; }

        }
        public class BuildParams
        {
            public string Name { get; set; }
            public string Version { get; set; }
            [JsonIgnore]
            public DateTime BuildDate { get; set; }
        }

        public class StaticParams
        {
            [JsonIgnore]
            public string Manufacturer { get; set; }
            public string ModelName { get; set; }
            public string Serial { get; set; }
            [JsonIgnore]
            public string[] Errors { get; set; }
        }

    }

    public class PeopleCounterLiveData
    {
        //[JsonProperty("name")]
        //public string Name { get; set; }
        [JsonProperty("in")]
        public int In { get; set; }
        [JsonProperty("out")]
        public int Out { get; set; }
    }

    public class TimedWebClient : WebClient
    {
        // Timeout in milliseconds, default = 600,000 msec
        public int Timeout { get; set; }

        public TimedWebClient()
        {
            this.Timeout = 600000;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var objWebRequest = base.GetWebRequest(address);
            objWebRequest.Timeout = this.Timeout;
            return objWebRequest;
        }
    }

    static class Helper
    {
        public static Dictionary<string, string> Dictionary(this string[] array, char separator)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var str in array)
            {
                var split = str.Split(separator);
                dict.Add(split[0], split[1]);
            }
            return dict;
        }
    }

}
