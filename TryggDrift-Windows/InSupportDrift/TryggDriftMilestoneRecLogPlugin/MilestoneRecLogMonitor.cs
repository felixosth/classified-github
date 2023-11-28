using InSupport.Drift.Monitor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace InSupport.Drift.Plugins
{
    public class MilestoneRecLogMonitor : BaseMonitor
    {
        public override float MonitorVersion => 1.4f;
        public override string MonitorName => "MilestoneRecLog";

        private string LogFilePath { get; set; } = @"C:\ProgramData\Milestone\XProtect Recording Server\Logs\DeviceHandling.log";

        private int DeviceComMinuteThreshold = 15;
        private const int OverflowMinuteThreshold = 20;

        private const string _OnlyOverflowSettingsKey = "MilestoneRecLogMode";

        bool checkOverflow = true;
        bool checkDeviceCom = true;

        //private MediaDbChecker mediaDbChecker;

        public MilestoneRecLogMonitor() : base()
        {

        }

        public override void LoadSettings(Dictionary<string, string> settings)
        {
            if (settings.ContainsKey("MilestoneRecLogThreshold"))
                int.TryParse(settings["MilestoneRecLogThreshold"], out DeviceComMinuteThreshold);

            if (settings.ContainsKey("MilestoneRecLogFilePath"))
                LogFilePath = settings["MilestoneRecLogFilePath"];

            if (settings.ContainsKey(_OnlyOverflowSettingsKey))
            {
                switch (settings[_OnlyOverflowSettingsKey].ToLower())
                {
                    case "overflow":
                        checkDeviceCom = false;
                        break;

                    case "devicecom":
                        checkOverflow = false;
                        break;

                    default:
                        checkOverflow = true;
                        checkDeviceCom = true;
                        break;
                }
            }

            //mediaDbChecker = new MediaDbChecker(settings);
        }

        public LogEntry[] InvalidLogs
        {
            get
            {
                if (!File.Exists(LogFilePath))
                    throw new FileNotFoundException("DeviceHandling.log does not exist at " + LogFilePath + ". Specify the correct path with the setting 'MilestoneRecLogFilePath'");

                var invalidLogs = new List<LogEntry>();

                if (checkDeviceCom)
                    invalidLogs.AddRange(DeviceComLogs());

                if (checkOverflow)
                    invalidLogs.AddRange(OverflowLogs());

                return invalidLogs.ToArray();
            }
        }

        //public RecServer RecServer => mediaDbChecker.GetRecServer();

        private List<LogEntry> OverflowLogs()
        {
            var deviceComlogs = GetLogEntries(LogFilePath, "overflow");
            var invalidLogs = new List<LogEntry>();
            LogEntry[] tmpInvLogs;

            for (int i = 0; i < deviceComlogs.Count; i++)
            {
                var log = deviceComlogs[i];

                if (new Regex("stream . in overflow").Match(log.Message).Success)
                {
                    invalidLogs.Add(log);
                    continue;
                }
                else if (new Regex("no longer in overflow").Match(log.Message).Success)
                {
                    tmpInvLogs = new LogEntry[invalidLogs.Count];
                    invalidLogs.CopyTo(tmpInvLogs);
                    foreach (var invLog in tmpInvLogs)
                    {
                        if (log.Item == invLog.Item)
                        {
                            invalidLogs.Remove(invLog);
                        }
                    }
                }
            }

            tmpInvLogs = new LogEntry[invalidLogs.Count];
            invalidLogs.CopyTo(tmpInvLogs);
            foreach (var invLog in tmpInvLogs)
            {
                var dif = DateTime.Now - invLog.TimeStamp;
                if (dif.TotalMinutes < OverflowMinuteThreshold)
                    invalidLogs.Remove(invLog);
            }

            if (invalidLogs.Count > 0)
            {
                invalidLogs = new List<LogEntry>()
                {
                    new LogEntry()
                    {
                        Message = "Cameras in overflow",
                        Category = "WARNING",
                        TimeStamp = DateTime.Now,
                        Item = "server"
                    }
                };
            }

            return invalidLogs;
        }

        private List<LogEntry> DeviceComLogs()
        {
            var deviceComlogs = GetLogEntries(LogFilePath, "Device communication");
            var invalidLogs = new List<LogEntry>();
            LogEntry[] tmpInvLogs;

            for (int i = 0; i < deviceComlogs.Count; i++)
            {
                var log = deviceComlogs[i];
                if (log.Message.Contains("Device communication error (NoDataException). Error: GetMediaDataBlock returned no data."))
                {
                    invalidLogs.Add(log);
                    continue;
                }
                else if (log.Message.Contains("Device communication established"))
                {
                    tmpInvLogs = new LogEntry[invalidLogs.Count];
                    invalidLogs.CopyTo(tmpInvLogs);
                    foreach (var invLog in tmpInvLogs)
                    {
                        if (log.Item == invLog.Item)
                        {
                            invalidLogs.Remove(invLog);
                        }
                    }
                }
            }

            tmpInvLogs = new LogEntry[invalidLogs.Count];
            invalidLogs.CopyTo(tmpInvLogs);
            foreach (var invLog in tmpInvLogs)
            {
                var dif = DateTime.Now - invLog.TimeStamp;
                if (dif.TotalMinutes < DeviceComMinuteThreshold)
                    invalidLogs.Remove(invLog);
            }

            return invalidLogs;
        }


        List<LogEntry> GetLogEntries(string file, string messageFilter)
        {
            List<LogEntry> logs = new List<LogEntry>();
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    while (!sr.EndOfStream)
                    {
                        try
                        {
                            string line = sr.ReadLine();
                            var timeStamp = DateTime.Parse(line.Split('[')[0].Trim());
                            var cat = line.Split(']')[1].Split('-')[0].Trim();

                            var firstRemoval = line.Remove(0, 52);

                            if (firstRemoval.StartsWith("****"))
                                continue;

                            var guid = firstRemoval.Remove(36, firstRemoval.Length - 36);
                            var msg = firstRemoval.Remove(0, 38);

                            if (!msg.Contains(messageFilter))
                                continue;

                            logs.Add(new LogEntry()
                            {
                                Item = guid,
                                Message = msg,
                                Category = cat,
                                TimeStamp = timeStamp
                            });
                        }
                        catch { }
                    }
                }
            }

            return logs;
        }
    }

    public class LogEntry
    {
        public DateTime TimeStamp { get; set; }

        public string Category { get; set; }

        public string Item { get; set; }

        public string Message { get; set; }

        public override string ToString() => string.Join(" ", Message.Replace("(NoDataException). Error: GetMediaDataBlock returned no data.", "").Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));
    }
}
