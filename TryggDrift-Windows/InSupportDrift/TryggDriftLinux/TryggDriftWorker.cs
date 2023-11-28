using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TryggDriftLinux.Monitor;

namespace TryggDriftLinux
{
    public class TryggDriftWorker : BackgroundService
    {
        const double _VERSION = 1.0;

        private readonly ILogger<TryggDriftWorker> _logger;
        private readonly WorkerOptions options;
        private readonly string machineId;

        public static DateTime LastCollect { get; private set; }


        List<BaseMonitorLinux> monitors;

        public TryggDriftWorker(ILogger<TryggDriftWorker> logger, WorkerOptions options)
        {
            _logger = logger;
            this.options = options;

            if (options.SendInterval < 60)
                options.SendInterval = 60;

            if (string.IsNullOrWhiteSpace(options.Hostname))
                options.Hostname = Dns.GetHostName();


            //var e = File.ReadAllText(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\hostnamectl").Split('\n').FirstOrDefault(r => r.Trim().StartsWith("Operating System"));


            machineId = ShellHelper.ReadFile("/var/lib/dbus/machine-id").Result;

            monitors = new List<BaseMonitorLinux>()
            {
                new PerformanceInfoLinuxMonitor(_logger),
                new SystemctlMonitor(_logger, options.MonitorServices ?? new string[0]),
                new DockerContainerMonitor(_logger, options.DockerContainers ?? new string[0])
            };

            _logger.LogDebug("Init with id {mid}", machineId);

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //_logger.LogInformation("Sending information to {address}", options.URL);
                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await SendData(stoppingToken);

                await Task.Delay(TimeSpan.FromSeconds(options.SendInterval), stoppingToken);
            }
        }

        private async Task SendData(CancellationToken stoppingToken)
        {
            var data = new
            {
                action = "reportIn",
                mguid = machineId,
                addCode = options.AddCode,
                monitors = GetSerializedMonitors(),
                version = _VERSION,
                name = options.Hostname
            };

            var payload = JsonConvert.SerializeObject(data);
            LastCollect = DateTime.Now;

            _logger.LogDebug("Sending the following data to TryggDrift: \"" + payload + "\"");

            using (var client = new HttpClient())
            {
                var content = new StringContent(payload, Encoding.UTF8, "application/json");
                var result = (await client.PostAsync(new Uri(new Uri(options.URL), "/backend/api.php"), content, stoppingToken)).Content.ReadAsStringAsync().Result;

                try
                {
                    JToken r = JsonConvert.DeserializeObject<JToken>(result);

                    if ((bool)r["success"] == true)
                    {
                        _logger.LogInformation("Data sent successfully.");
                    }
                    else
                    {
                        _logger.LogError("Data sent unsuccessfully, response: \"" + result + "\"");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error deserializing response: " + ex.Message);
                }

                _logger.LogDebug("Data sent, result: \"{result}\". Waiting {sec} seconds", result, options.SendInterval);
            }
        }

        private string GetSerializedMonitors()
        {
            return JsonConvert.SerializeObject(monitors);
        }
    }

    public static class ShellHelper
    {
        public static string Bash(this string cmd)
        {
            var escapedArgs = cmd.Replace("\"", "\\\"");

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return result;
        }

        public static async Task<string> ReadFile(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 512, FileOptions.SequentialScan | FileOptions.Asynchronous))
            using (var r = new StreamReader(fs, Encoding.ASCII))
            {
                return await r.ReadToEndAsync();
            }
        }

        public static async Task<string> ReadLineStartingWithAsync(string path, string lineStartsWith)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 512, FileOptions.SequentialScan | FileOptions.Asynchronous))
            using (var r = new StreamReader(fs, Encoding.ASCII))
            {
                string line;
                while ((line = await r.ReadLineAsync()) != null)
                {
                    if (line.StartsWith(lineStartsWith))
                        return line;
                }
            }

            return null;
        }
    }
}
