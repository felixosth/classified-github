using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TryggSTORE.Http.HTML;
using TryggSTORE.Http.Video;
using TryggSTORE.MJPEG;
using TryggSTORE.MJPEG.FrameCreators;

namespace TryggSTORE.Http
{
    public class TryggSTOREServer
    {
        private readonly HttpListener httpListener;
        private readonly HttpApiStream apiStream;
        internal List<MJPEGStream> ActiveMJPEGStreams { get; private set; }

        private readonly OccupancyFrameCreator frameCreator;
        private bool running = false;

        private readonly NotifyManager notifyManager;
        private readonly OccupancyCommuncation occupancyCommuncation;

        public event EventHandler<Exception> OnError;
        public event EventHandler<string> OnLog;

        public TryggSTOREServer(int port)
        {
            httpListener = new HttpListener();
            apiStream = new HttpApiStream(this);
            ActiveMJPEGStreams = new List<MJPEGStream>();

            httpListener.Prefixes.Add($"http://+:{port}/");

            frameCreator = new OccupancyFrameCreator();
            notifyManager = new NotifyManager(ConfigFile.Instance.NotifyProfiles);
            notifyManager.OnError += Logger_OnError;
            occupancyCommuncation = new OccupancyCommuncation();
        }

        public void Start()
        {
            occupancyCommuncation.Start();
            frameCreator.OnError += Logger_OnError;
            frameCreator.StartCreate();

            try
            {
                httpListener.Start();
                running = true;
                new Thread(ListenThread) { Name = "Http listen thread" }.Start();
            }
            catch(Exception e)
            {
                OnError?.Invoke(this, e);
            }

            notifyManager.Start();
        }

        private void Logger_OnError(object sender, string e)
        {
            Log(sender + ": " + e);
        }

        void ListenThread()
        {
            while (running)
            {
                try
                {
                    HttpListenerContext context = httpListener.GetContext();
                    new Thread(NewContext) { IsBackground = true, Name = context.Request.RemoteEndPoint.ToString() + " http thread" }.Start(context);

                }
                catch(Exception e)
                {
                    OnError?.Invoke(this, e);
                }
            }
        }

        void NewContext(object context)
        {
            try
            {
                HttpListenerContext ctx = context as HttpListenerContext;
                Log(string.Format("{0} {1}", ctx.Request.RemoteEndPoint.ToString(), ctx.Request.Url.ToString()));

                HttpListenerRequest request = ctx.Request;

                if (request.Url.Segments.Length > 1)
                {
                    var requestedResource = request.Url.Segments[1];

                    try
                    {
                        switch (requestedResource)
                        {
                            case "stream.mjpeg":
                                StartStream(ctx, "stream");
                                break;
                            case "api":
                                apiStream.HandleApiRequest(ctx);
                                break;
                            default:
                                HttpFileStream.HandleRequest(ctx);
                                break;
                        }
                    }
                    catch
                    {
                        throw;
                    }
                }
                else
                {
                    // redirect to status
                    ctx.Response.StatusCode = 301;
                    ctx.Response.RedirectLocation = "/status.html";
                    ctx.Response.OutputStream.Close();
                }
            }
            catch (Exception e)
            {
                OnError?.Invoke(this, e);
            }
        }

        private void StartStream(HttpListenerContext ctx, string requestedFrameCreator)
        {
            Log("Connecting " + ctx.Request.RemoteEndPoint.ToString() + " to \"" + requestedFrameCreator + "\"");
            lock (ActiveMJPEGStreams)
            {
                var stream = new MJPEGStream(ctx, frameCreator);
                stream.OnError += Stream_OnError;
                stream.OnDisconnect += Stream_OnDisconnect;
                ActiveMJPEGStreams.Add(stream);
                stream.Start();
            }
        }

        public void Stop()
        {
            notifyManager.Stop();
            ConfigFile.Instance.SaveConfig();
            running = false;
            httpListener.Stop();
        }

        private void Stream_OnDisconnect(object sender, EventArgs e)
        {
            try
            {
                var stream = sender as MJPEGStream;
                Log(stream.Ctx.Request.RemoteEndPoint.ToString() + " disconnected.");
                lock (ActiveMJPEGStreams)
                {
                    stream.OnDisconnect -= Stream_OnDisconnect;
                    stream.OnError -= Stream_OnError;
                    ActiveMJPEGStreams.Remove(stream);
                }
            }
            catch(Exception ex)
            {
                OnError?.Invoke(this, ex);
            }
        }

        private void Stream_OnError(object sender, string e)
        {
            OnError?.Invoke(this, new Exception(e));

            Log("StreamError: " + e);
        }

        private void Log(string msg)
        {
            OnLog?.Invoke(this, msg);
        }
    }

}
