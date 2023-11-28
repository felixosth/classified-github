using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static TryggDriftLinux.ShellHelper;

namespace TryggDriftLinux.Monitor
{
    internal class PerformanceInfoLinuxMonitor : BaseMonitorLinux
    {
        private const string MEMINFO_FILEPATH = "/proc/meminfo";
        private const string CPUSTAT_FILEPATH = "/proc/stat";

        private List<LinuxInterface> interfaces;

        public PerformanceInfoLinuxMonitor(ILogger<TryggDriftWorker> logger) : base(logger)
        {

            interfaces = new List<LinuxInterface>();


            foreach (var interfaceStr in GetInterfaces())
            {
                interfaces.Add(new LinuxInterface(interfaceStr));
            }
        }

        public override string MonitorName => "PerformanceInfoLinux";

        public override float MonitorVersion => 1;


        private long totalMem = -1;
        public long? TotalMem
        {
            get
            {

                if (totalMem == -1)
                {


                    var memTotalLine = ReadLineStartingWithAsync(MEMINFO_FILEPATH, "MemTotal").Result;

                    if (string.IsNullOrWhiteSpace(memTotalLine))
                    {
                        Logger.LogWarning($"Couldn't read 'MemTotal' line from '{MEMINFO_FILEPATH}'");
                        return null;
                    }

                    // Format: "MemTotal:       16426476 kB"
                    if (!long.TryParse(new string(memTotalLine.Where(char.IsDigit).ToArray()), out var totalMemInKb))
                    {
                        Logger.LogWarning($"Couldn't parse meminfo output");
                        return null;
                    }

                    totalMem = totalMemInKb * 1_000;
                }
                return totalMem;
            }
        }

        public long? AvailableMem
        {
            get
            {
                var memAvailableLine = ReadLineStartingWithAsync(MEMINFO_FILEPATH, "MemAvailable").Result;

                if (string.IsNullOrWhiteSpace(memAvailableLine))
                {
                    memAvailableLine = ReadLineStartingWithAsync(MEMINFO_FILEPATH, "MemFree").Result;

                    if (string.IsNullOrWhiteSpace(memAvailableLine))
                    {
                        Logger.LogWarning($"Couldn't read 'MemAvailable' or 'MemFree' line from '{MEMINFO_FILEPATH}'");
                        return null;
                    }
                }

                if (!long.TryParse(new string(memAvailableLine.Where(char.IsDigit).ToArray()), out var availableMemInKb))
                {
                    Logger.LogWarning($"Couldn't parse meminfo output: '{memAvailableLine}'");
                    return null;
                }
                return availableMemInKb * 1_000;
            }
        }

        private long prevUserTime, prevNiceTime, prevSystemTime, prevIrqTime, prevSoftirqTime, prevStealTime, prevIdleTime, prevIowaitTime;

        public double? CpuUsage
        {
            get
            {
                var cpuUsageLine = ReadLineStartingWithAsync(CPUSTAT_FILEPATH, "cpu  ").Result;

                if (string.IsNullOrWhiteSpace(cpuUsageLine))
                {
                    Logger.LogWarning($"Couldn't read line from '{CPUSTAT_FILEPATH}'");
                    return null;
                }

                // Format: "cpu  20546715 4367 11631326 215282964 96602 0 584080 0 0 0"
                var cpuNumberStrings = cpuUsageLine.Split(' ').Skip(2);

                if (cpuNumberStrings.Any(n => !long.TryParse(n, out _)))
                {
                    Logger.LogWarning($"Failed to parse '{CPUSTAT_FILEPATH}' output correctly. Line: {cpuUsageLine}");
                    return null;
                }

                var cpuNumbers = cpuNumberStrings.Select(long.Parse).ToArray();

                var userTime = cpuNumbers[0];
                var niceTime = cpuNumbers[1];
                var systemTime = cpuNumbers[2];
                var idleTime = cpuNumbers[3];
                var iowaitTime = cpuNumbers[4]; // Iowait is not real cpu time
                var irqTime = cpuNumbers[5];
                var softirqTime = cpuNumbers[6];
                var stealTime = cpuNumbers[7];

                var prevIdle = prevIdleTime + prevIowaitTime;
                var idle = idleTime + iowaitTime;

                var prevNonIdle = prevUserTime + prevNiceTime + prevSystemTime + prevIrqTime + prevSoftirqTime + prevStealTime;
                var nonIdle = userTime + niceTime + systemTime + irqTime + softirqTime + stealTime;

                var prevTotal = prevIdle + prevNonIdle;

                var total = idle + nonIdle;

                var totald = total - prevTotal;
                var idled = idle - prevIdle;

                var percentage = (totald - idled) / (double)totald;
                //Logger.LogInformation("CPU: {cpu}%", percentage);

                prevUserTime = userTime;
                prevNiceTime = niceTime;
                prevSystemTime = systemTime;
                prevIdleTime = idleTime;
                prevIowaitTime = iowaitTime;
                prevIrqTime = irqTime;
                prevSoftirqTime = softirqTime;
                prevStealTime = stealTime;

                return Math.Round(percentage * 100, 1);
            }
        }

        public int Uptime
        {
            get
            {
                return (int)double.Parse(ReadFile("/proc/uptime").Result.Split(' ')[0]);
            }
        }

        private string os = null;
        public string OS
        {
            get
            {
                if (os == null)
                    os = "lsb_release -a".Bash().Split("\n").FirstOrDefault(s => s.StartsWith("Description")).Remove(0, "Description:\t".Length);

                return os ?? System.Runtime.InteropServices.RuntimeInformation.OSDescription;
                //return System.Runtime.InteropServices.RuntimeInformation.OSDescription;

            }
        }


        private string _processorName = null;
        public string ProcessorName
        {
            get
            {
                if (_processorName == null)
                    _processorName = "cat /proc/cpuinfo  | grep 'name'| uniq".Bash().Remove(0, "model name\t: ".Length);

                return _processorName;
            }
        }

        private int _processors = -1;
        public int Processors
        {
            get
            {
                if (_processors == -1)
                {
                    var processorsStr = "cat /proc/cpuinfo  | grep process| wc -l".Bash();
                    int processors = 0;
                    if (int.TryParse(processorsStr, out processors))
                    {
                        _processors = processors;
                    }
                    else
                    {
                        Logger.LogWarning("Unable to parse processors");
                    }
                }

                return _processors;
            }
        }


        bool useGpu = true;

        public IEnumerable<NvidiaGPU> GPUs
        {
            get
            {
                if (!useGpu)
                    return new NvidiaGPU[0];

                var gpusStr = "nvidia-smi --query-gpu=timestamp,name,driver_version,temperature.gpu,utilization.gpu,memory.total,memory.free,memory.used --format=csv".Bash();

                try
                {
                    using (var reader = new StringReader(gpusStr))
                    using (var csvReader = new CsvReader(reader, new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
                    {
                        PrepareHeaderForMatch = args => args.Header.ToLower(),
                        Delimiter = ", "
                    }))
                    {
                        return csvReader.GetRecords<NvidiaGPU>().ToList();
                    }
                }
                catch
                {
                    if (useGpu)
                    {
                        Logger.LogDebug("nvidia-smi error, ignoring gpu..");
                        useGpu = false;
                    }

                    return new NvidiaGPU[0];
                }
            }
        }

        bool first = true;
        public IEnumerable<LinuxInterface> Interfaces
        {
            get
            {
                var now = DateTime.Now;
                var secondsSinceLastCollect = (now - TryggDriftWorker.LastCollect).TotalSeconds;
                foreach (var _interface in interfaces)
                {
                    _interface.FirstCall = first;
                    var rx = long.Parse(ReadFile($"/sys/class/net/{_interface.Name}/statistics/rx_bytes").Result);
                    var tx = long.Parse(ReadFile($"/sys/class/net/{_interface.Name}/statistics/tx_bytes").Result);


                    _interface.LastRbytes = _interface.RBytes;
                    _interface.RBytes = rx - _interface.LastRbytes;

                    _interface.LastTbytes = _interface.TBytes;
                    _interface.TBytes = tx - _interface.LastTbytes;

                    if (!first)
                    {
                        _interface.RBytesPerSec = (int)(_interface.RBytes / secondsSinceLastCollect);
                        _interface.TBytesPerSec = (int)(_interface.TBytes / secondsSinceLastCollect);
                    }
                }

                first = false;
                return interfaces;
            }
        }

        private string[] GetInterfaces()
        {
            return "ls /sys/class/net".Bash().Split("\n", StringSplitOptions.RemoveEmptyEntries);
        }
    }

    public class NvidiaGPU
    {
        [CsvHelper.Configuration.Attributes.Name("timestamp")]
        public DateTime Timestamp { get; set; }

        [CsvHelper.Configuration.Attributes.Name("name")]
        public string Name { get; set; }

        [CsvHelper.Configuration.Attributes.Name("driver_version")]
        public string Driver { get; set; }

        [CsvHelper.Configuration.Attributes.Name("temperature.gpu")]
        public int Temperature { get; set; }

        [CsvHelper.Configuration.Attributes.Name("utilization.gpu [%]")]
        public string Utilization { get; set; }

        [CsvHelper.Configuration.Attributes.Name("memory.total [MiB]")]
        public string MemTotal { get; set; }

        [CsvHelper.Configuration.Attributes.Name("memory.free [MiB]")]
        public string MemFree { get; set; }

        [CsvHelper.Configuration.Attributes.Name("memory.free [MiB]")]
        public string MemUsed { get; set; }
    }

    public class LinuxInterface
    {
        [JsonIgnore]
        public bool FirstCall = true;
        public string Name { get; set; }

        [JsonIgnore]
        public long LastRbytes { get; set; }

        private long rbytes;
        public long RBytes { get => FirstCall ? 0 : rbytes; set => rbytes = value; }

        public int RBytesPerSec { get; set; }
        public int TBytesPerSec { get; set; }

        [JsonIgnore]
        public long LastTbytes { get; set; }
        private long tbytes;
        public long TBytes { get => FirstCall ? 0 : tbytes; set => tbytes = value; }

        public LinuxInterface(string name)
        {
            this.Name = name;
        }
    }
}
