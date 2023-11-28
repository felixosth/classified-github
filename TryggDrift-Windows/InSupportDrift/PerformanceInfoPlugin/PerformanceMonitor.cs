using InSupport.Drift.Monitor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Threading;
using InSupport.Drift.Plugins.PerformanceInfo;

namespace InSupport.Drift.Plugins
{
    public class PerformanceMonitor : BaseMonitor
    {
        public override float MonitorVersion => 1.1f;
        private CpuInfo CPU => new CpuInfo();
        private RamInfo RAM => new RamInfo();
        private OSInfo OS => new OSInfo();

        private NicMonitor[] Nics = NicMonitor.GetNics();

        public object System
        {
            get
            {
                return new
                {
                    CPU,
                    RAM,
                    OS,
                    Nics
                };
            }
        }

        public PerformanceMonitor() : base()
        {
        }

        public override void LoadSettings(Dictionary<string, string> settings)
        {
        }

        public override string MonitorName => "PerformanceInfo";
    }

    class StorageInfo
    {
        public Drive[] Drives
        {
            get
            {
                //criticalStorage = "";
                List<Drive> storageInfos = new List<Drive>();

                var counterCat = new PerformanceCounterCategory("PhysicalDisk");
                var instNames = counterCat.GetInstanceNames();

                foreach (DriveInfo drive in System.IO.DriveInfo.GetDrives())
                {
                    if (!drive.IsReady)
                        continue;

                    //bool found = false;
                    //foreach (var d in drives)
                    //{
                    //    found = drive.Name.StartsWith(d);
                    //    if (found)
                    //        break;
                    //}
                    //if (!found)
                    //    continue;

                    PerformanceCounter[] myCounters = null;

                    foreach (var instance in instNames)
                    {
                        if (instance[2] == drive.Name[0])
                        {
                            myCounters = counterCat.GetCounters(instance);

                            //myCounter = new PerformanceCounter(cat, "% Disk Time", instance);
                        }
                    }

                    storageInfos.Add(new Drive(drive, myCounters));
                }
                return storageInfos.ToArray();
            }
        }

        public class Drive
        {
            public string Label { get; set; }
            public string Name { get; set; }
            public double PercentageFree { get; set; }
            public long FreeSpace { get; set; }
            public long TotalSpace { get; set; }

            //public float AverageDiskUsagePercent { get; set; }
            //public Dictionary<string, float> CounterValues { get; set; }

            public Drive(DriveInfo drive, PerformanceCounter[] counters)
            {
                this.Label = drive.VolumeLabel;
                this.Name = drive.Name;
                this.FreeSpace = drive.TotalFreeSpace;
                this.TotalSpace = drive.TotalSize;
                this.PercentageFree = ToPercentage(drive.TotalFreeSpace, drive.TotalSize);

                //CounterValues = Getter.GetAllCounterValues(counters, 10);
            }


            static double ToPercentage(long avalible, long total)
            {
                return ((double)avalible / (double)total * 100);
            }
        }
    }

    class OSInfo
    {

        public OSInfo()
        {
            var cs = Getter.Query("Win32_OperatingSystem");
            try
            {
                foreach (var obj in cs)
                {
                    InstallDate = ManagementDateTimeConverter.ToDateTime(obj["InstallDate"].ToString());
                    LastBootUp = ManagementDateTimeConverter.ToDateTime(obj["LastBootUpTime"].ToString());
                    Name = obj["Caption"].ToString();

                    break;
                }
            }
            catch { }
        }

        public string Name { get; set; }

        public DateTime InstallDate { get; set; }
        public DateTime LastBootUp { get; set; }

        public float UptimePercentage
        {
            get
            {
                try
                {
                    EventLog eventLog = new EventLog("system");

                    var entries = eventLog.Entries.Cast<EventLogEntry>().Where(e => (e.InstanceId & 0x3FFFFFFF) == 6005 || (e.InstanceId & 0x3FFFFFFF) == 6006 || (e.InstanceId & 0x3FFFFFFF) == 6008).ToList();

                    var totalInterval = new TimeSpan();
                    for (int i = 0; i < entries.Count; i++)
                    {
                        var myEventId = entries[i].InstanceId & 0x3FFFFFFF;

                        if (i >= entries.Count - 1)
                        {
                            if (myEventId == 6005)
                            {
                                totalInterval += DateTime.Now - entries[i].TimeWritten;
                            }
                        }
                        else
                        {
                            var nextEventId = entries[i + 1].InstanceId & 0x3FFFFFFF;

                            if (myEventId == 6005 && nextEventId == 6006)
                            {
                                totalInterval += entries[i + 1].TimeGenerated - entries[i].TimeGenerated;
                            }
                            else if (myEventId == 6005 && nextEventId == 6008)
                            {
                                var nextEvent = entries[i + 1];

                                var dateString = nextEvent.ReplacementStrings[1] + " " + nextEvent.ReplacementStrings[0];  // = 2018-‎09-‎27 16:16:45

                                var cleanString = new string(dateString.ToCharArray().Where(c => c != 8206).ToArray());

                                var culture = Thread.CurrentThread.CurrentCulture;
                                var format = culture.DateTimeFormat.ShortDatePattern + " " + culture.DateTimeFormat.LongTimePattern; // = "yyyy-MM-dd HH:mm:ss"

                                DateTime dateTime = DateTime.ParseExact(cleanString, format, culture);
                                totalInterval += dateTime - entries[i].TimeWritten;
                            }
                        }
                    }

                    var totalTime = DateTime.Now - entries[0].TimeWritten;
                    return (float)(totalInterval.TotalSeconds / totalTime.TotalSeconds * 100);
                }
                catch
                {
                    return 0;
                }
            }
        }

    }

    class RamInfo
    {
        public long TotalMemory
        {
            get
            {
                var cs = Getter.Query("Win32_ComputerSystem");
                try
                {
                    foreach (var obj in cs)
                    {
                        return long.Parse(obj["TotalPhysicalMemory"].ToString());
                    }
                }
                catch { }

                return 0;
            }
        }

        public long AvailableMemory
        {
            get
            {
                var cs = Getter.Query("Win32_PerfFormattedData_PerfOS_Memory");
                try
                {
                    foreach (var obj in cs)
                    {
                        return long.Parse(obj["AvailableBytes"].ToString());
                    }
                }
                catch { }

                return 0;
            }
        }
    }

    class CpuInfo
    {
        public string Name { get; set; }
        public float LoadPercent { get; set; }

        public CpuInfo()
        {
            LoadPercent = 0;
            try
            {
                for (int i = 0; i < 5; i++)
                {
                    var processors = Getter.Query("Win32_Processor");
                    foreach (var cpu in processors)
                    {

                        Name = cpu["Name"].ToString();
                        LoadPercent += float.Parse(cpu["LoadPercentage"].ToString());

                        break;
                    }
                    Thread.Sleep(250);
                }
                LoadPercent /= 5;
            }
            catch { }
        }
    }

    public static class Getter
    {
        public static ManagementObjectCollection Query(string q, string path = "root\\CIMV2")
        {
            return new ManagementObjectSearcher(path, "SELECT * FROM " + q).Get();
        }

        public static Dictionary<string, float> GetAllCounterValues(PerformanceCounter[] counters, int checks)
        {
            Dictionary<string, float> counterValues = new Dictionary<string, float>();

            foreach (var counter in counters)
            {
                counter.NextValue();
            }

            for (int i = 0; i < checks; i++)
            {
                foreach (var counter in counters)
                {
                    if (!counterValues.ContainsKey(counter.CounterName))
                    {
                        counterValues[counter.CounterName] = 0;
                    }

                    counterValues[counter.CounterName] += counter.NextValue();
                }
                //total += counter.NextValue();
                Thread.Sleep(350);
            }

            Dictionary<string, float> finalValues = new Dictionary<string, float>();

            foreach (var cv in counterValues)
            {
                finalValues[cv.Key] = cv.Value / checks;
            }
            return finalValues;
        }
    }
}
