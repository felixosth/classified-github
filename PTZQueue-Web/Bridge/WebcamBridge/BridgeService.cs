using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebcamBridge
{
    public partial class BridgeService : ServiceBase
    {
        private WebServer webServer;
        private Dictionary<string, string> config;
        public bool Running { get; private set; }

        private WebClient GetWC() => new WebClient() { Credentials = new NetworkCredential(config[Constants.CameraUsername], config[Constants.CameraPassword]) };

        public BridgeService()
        {
            InitializeComponent();

            #region eventvwr
            this.ServiceName = Constants.ServiceName;
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

            config = Config.GetConfig();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            if(config.ContainsKey(Constants.TrustAllCertificates) && bool.Parse(config[Constants.TrustAllCertificates]))
            {
                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            }

            if(config.ContainsKey(Constants.LaunchDebugger) && bool.Parse(config[Constants.LaunchDebugger]))
            {
                Debugger.Launch();
            }

            webServer = new WebServer(new string[] { $"http://{config[Constants.ServerBind]}:{config[Constants.ServerPort]}/" }, HttpCallback, LogCallback);
            webServer.LogOnEachRequest = false;
        }

        private void Log(string msg, EventLogEntryType logType = EventLogEntryType.Information, int eventId = 0)
        {
            try
            {
                this.EventLog.WriteEntry(msg, logType, eventId);
            }
            catch { }
        }

        void LogCallback(string msg)
        {
            Log(msg, eventId: 1);
        }

        private void HttpCallback(HttpListenerContext ctx)
        {
            if (ctx.Request.Url.Segments.Length > 1)
            {
                if (ctx.Request.Url.Segments[1].ToLower() == "image")
                    ProcessRequestImage(ctx);
                else if (ctx.Request.Url.Segments[1].ToLower() == "ptz")
                    ProcessRequestPtz(ctx);
                else if (ctx.Request.Url.Segments[1].ToLower() == "getptz")
                    ProcessGetPTZ(ctx);
                else
                {
                    ctx.Response.StatusCode = 404;
                }
            }
            else
            {
                ctx.Response.StatusCode = 404;
            }
        }

        private void ProcessGetPTZ(HttpListenerContext ctx)
        {
            string result = null;
            using (var wc = GetWC())
                result = wc.DownloadString($"{config[Constants.CameraAddress]}/axis-cgi/com/ptz.cgi?query=position");

            Dictionary<string, string> values = new Dictionary<string, string>();
            string[] rows = result.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach(var row in rows)
            {
                var rowSplit = row.Split('=');
                values.Add(rowSplit[0], rowSplit[1]);
            }

            ctx.Response.Headers.Add("Access-Control-Allow-Origin: *");
            ctx.Response.ContentType = "text/plain";

            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(values));
            using (var stream = ctx.Response.OutputStream)
            {
                stream.Write(bytes, 0, bytes.Length);
            }
        }

        private void ProcessRequestPtz(HttpListenerContext ctx)
        {
            ctx.Response.Headers.Add("Access-Control-Allow-Origin: *");
            ctx.Response.StatusCode = 200;
            ctx.Response.OutputStream.Close();

            using (var wc = GetWC())
                wc.DownloadString($"{config[Constants.CameraAddress]}/axis-cgi/com/ptz.cgi" + ctx.Request.Url.Query);
        }

        private void ProcessRequestImage(HttpListenerContext ctx)
        {
            ctx.Response.Headers.Clear();
            foreach (String hdrName in headers.AllKeys)
            {
                if (hdrName == "Content-Type" || hdrName == "Content-Length" || hdrName == "Connection"
                    || hdrName == "Transfer-Encoding")
                    continue;
                ctx.Response.Headers.Add(hdrName, headers.Get(hdrName));
            }

            ctx.Response.ContentType = "image/jpeg";

            using (var stream = ctx.Response.OutputStream)
            {
                stream.Write(imageBytes, 0, imageBytes.Length);
            }
        }

        WebHeaderCollection headers;
        static byte[] imageBytes;
        private void FetchImage(string url)
        {
            var req = WebRequest.CreateHttp(url);

            req.Credentials = new NetworkCredential(config[Constants.CameraUsername], config[Constants.CameraPassword]);

            using (var resp = req.GetResponse())
            {
                headers = resp.Headers;

                using (var input = resp.GetResponseStream())
                using (var ms = new MemoryStream())
                {
                    input.CopyTo(ms);

                    imageBytes = ms.ToArray();
                }
            }
        }

        private void FetchImageThread()
        {
            bool firstGet = true;
            int sleepInterval = int.Parse(config[Constants.CameraImageFetchIntervalMs]);
            string imageFetchUrl = $"{config[Constants.CameraAddress]}/jpg/image.jpg?resolution={config[Constants.CameraResolution]}";
            try
            {
                while (Running)
                {
                    FetchImage(imageFetchUrl);
                    if(firstGet)
                    {
                        Log($"Image fetch successful from {imageFetchUrl}. ({imageBytes.Length/1024} kilobytes)", eventId: 2);
                        firstGet = false;
                    }
                    Thread.Sleep(sleepInterval);
                }
            }
            catch(Exception ex)
            {
                Log(ex.ToString(), EventLogEntryType.Error, eventId: 3);
            }
        }

        protected override void OnStart(string[] args)
        {
            Running = true;
            new Thread(FetchImageThread).Start();
            webServer.Start();
        }

        protected override void OnStop()
        {
            Running = false;
            webServer.Stop();
        }

        public void Debug()
        {
            OnStart(new string[0]);
        }
    }
}
