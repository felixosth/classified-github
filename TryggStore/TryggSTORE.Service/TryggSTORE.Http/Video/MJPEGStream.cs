using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TryggSTORE.MJPEG.FrameCreators;

namespace TryggSTORE.MJPEG
{
    internal class MJPEGStream
    {
        private long bytesSent = 0;

        public FrameCreatorBase FrameCreator { get; private set; }
        const string _HttpBoundary = "MJPEGStreamerboundary";

        internal HttpListenerContext Ctx { get; private set; }

        internal string Client
        {
            get
            {
                try
                {
                    return Ctx.Request.RemoteEndPoint.ToString();
                }
                catch
                {
                    return "N/A";
                }
            }
        }
        Stream output { get; set; }

        private byte[] imageData => FrameCreator.LatestFrame;
        //private byte[] imageData { get; set; }
        object imageLock = new object();

        public long KbpsBandwidth { get; private set; }

        public event EventHandler<string> OnError;
        public event EventHandler OnDisconnect;

        internal MJPEGStream(HttpListenerContext context, FrameCreatorBase frameCreator)
        {
            //frameCreator = new MJPEGFrameCreator();
            //frameCreator.OnError += FrameCreator_OnError;
            this.FrameCreator = frameCreator;
            FrameCreator.OnError += FrameCreator_OnError;
            frameCreator.OnNewImageAvailable += FrameCreator_OnNewImageAvailable;
            //frameCreator.StartCreate();
            SetImage(frameCreator.LatestFrame);

            Ctx = context;
            output = Ctx.Response.OutputStream;
        }

        private void FrameCreator_OnError(object sender, string e)
        {
            OnError?.Invoke(sender, e);
        }

        private void FrameCreator_OnNewImageAvailable(object sender, EventArgs e)
        {
            SetImage((sender as FrameCreatorBase).LatestFrame);
        }

        public void Start()
        {
            HttpListenerResponse response = Ctx.Response;

            if(Ctx.Request.HttpMethod.ToUpper() == "HEAD")
            {
                response.Headers.Add("Cache-Control", "no-cache");
                response.Headers.Add("Pragma", "no-cache");
                response.ContentType = $"multipart/x-mixed-replace;boundary={_HttpBoundary}";
                response.Headers.Add("Server", "MJPEGServer/0.1");
                output.Close();
                OnDisconnect?.Invoke(this, new EventArgs());
                return;
            }
            else
            {
            }

            // Write initial header
            response.Headers.Add("Cache-Control", "no-cache");
            response.Headers.Add("Pragma", "no-cache");
            response.ContentType = $"multipart/x-mixed-replace;boundary={_HttpBoundary}";
            response.Headers.Add("Server", "MJPEGServer/0.1");

            Write("\\n--" + _HttpBoundary + "\r\n");

            new Thread(HandleStream)
            {
                IsBackground = true,
                Name = Ctx.Request.RemoteEndPoint.ToString() + " mjpeg thread"
            }.Start();
        }

        private void SetImage(byte[] newImageData)
        {
            lock(imageLock)
            {
                //imageData = newImageData;
            }
        }

        private void HandleStream()
        {
            try
            {
                int lastWrittenHash = 0;

                DateTime lastSend = DateTime.Now;

                while (true)
                {
                    if(imageData == null)
                    {
                        Thread.Sleep(100);
                        continue;
                    }

                    int newWriteHash = imageData.GetHashCode();

                    if (newWriteHash != lastWrittenHash)
                    {

                        lastWrittenHash = newWriteHash;

                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("Mime-Type: image/jpeg");
                        sb.AppendLine("Content-Type: image/jpeg");

                        sb.AppendLine("Content-Length: " + imageData.Length);
                        sb.AppendLine();

                        Write(sb.ToString());
                        Write(imageData);

                        Write("\r\n--" + _HttpBoundary + "\r\n");

                        //Console.WriteLine("Written {0} bytes", bytesSent);

                        var now = DateTime.Now;
                        long bits = bytesSent * 8;
                        var bitsPerSec = bits / (now - lastSend).TotalSeconds;

                        KbpsBandwidth = (long)(bitsPerSec / 1024);
                        if (KbpsBandwidth < 0)
                            KbpsBandwidth = 0;

                        bytesSent = 0;
                        lastSend = now;
                    }

                    Thread.Sleep(50);
                }
            }
            catch(HttpListenerException ex)
            {
                //OnError?.Invoke(this, ex.Message);
                OnDisconnect?.Invoke(this, new EventArgs());
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, ex.Message);
                OnDisconnect?.Invoke(this, new EventArgs());
            }
        }

        private void Write(string text)
        {
            byte[] data = Encoding.ASCII.GetBytes(text);

            output.Write(data, 0, data.Length);

            bytesSent += data.Length;
        }

        private void Write(byte[] data)
        {
            output.Write(data, 0, data.Length);
            bytesSent += data.Length;
        }



    }
}
