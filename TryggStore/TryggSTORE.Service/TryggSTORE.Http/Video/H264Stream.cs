using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TryggSTORE.MJPEG.FrameCreators;

namespace TryggSTORE.Http.Video
{
    internal class H264Stream
    {
        private long bytesSent = 0;
        private HttpListenerContext Ctx;
        const string _HttpBoundary = "H264Streamerboundary";
        Stream output { get; set; }

        public long KbpsBandwidth { get; private set; }
        private byte[] imageData => FrameCreator.LatestFrame;

        public event EventHandler<string> OnError;
        public event EventHandler OnDisconnect;
        public FrameCreatorBase FrameCreator { get; private set; }

        internal H264Stream(HttpListenerContext context, FrameCreatorBase frameCreator)
        {
            Ctx = context;
            FrameCreator = frameCreator;
            output = Ctx?.Response.OutputStream;
        }

        public void Start()
        {
            HttpListenerResponse response = Ctx.Response;

            if (Ctx.Request.HttpMethod.ToUpper() == "HEAD")
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

            var ffmpeg = GetFFMPEGProcess("-i - -c:v libx264 -f mp4 -");

            ffmpeg.Start();
            ffmpeg.BeginErrorReadLine();

            var streamParams = new StreamObjectParams()
            {
                Input = ffmpeg.StandardInput.BaseStream,
                Output = ffmpeg.StandardOutput.BaseStream
            };

            new Thread(HandlStream) { IsBackground = true }.Start(streamParams);
            new Thread(() =>
            {
                int lastWrittenHash = 0;
                while (true)
                {
                    int newWriteHash = imageData.GetHashCode();
                    if (newWriteHash != lastWrittenHash)
                    {
                        lastWrittenHash = newWriteHash;
                        streamParams.Input.Write(imageData, 0, imageData.Length);
                    }
                }
            }) { IsBackground = true }.Start();
        }
        
        public void Test()
        {
            var ffmpeg = GetFFMPEGProcess("-re -i - -c:v libx264 output.h264");

            ffmpeg.Start();
            ffmpeg.BeginErrorReadLine();

            var input = ffmpeg.StandardInput.BaseStream;

            int lastWrittenHash = 0;
            while (true)
            {
                if(imageData == null)
                {
                    Thread.Sleep(50);
                    continue;
                }    
                int newWriteHash = imageData.GetHashCode();
                if (newWriteHash != lastWrittenHash)
                {
                    lastWrittenHash = newWriteHash;
                    input.Write(imageData, 0, imageData.Length);
                }
                Thread.Sleep(50);
            }
        }


        private void HandlStream(object streamObj)
        {
            var streamParams = streamObj as StreamObjectParams;
            byte[] buffer = new byte[8192];

            while (true)
            {
                if (imageData == null)
                {
                    Thread.Sleep(100);
                    continue;
                }

                streamParams.Output.Read(buffer, 0, buffer.Length);

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Mime-Type: image/x-h264");
                sb.AppendLine("Content-Type: image/x-h264");

                sb.AppendLine("Content-Length: " + buffer.Length);
                sb.AppendLine();

                Write(sb.ToString());
                Write(buffer);

                Write("\r\n--" + _HttpBoundary + "\r\n");
            }
        }

        private Process GetFFMPEGProcess(string args)
        {
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Video\\ffmpeg\\ffmpeg.exe"), args);

            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = true;
            p.ErrorDataReceived += (s, e) => System.Diagnostics.Debug.WriteLine(e.Data);

            //p.Start();
            return p;
        }

        internal class StreamObjectParams
        {
            internal Stream Input { get; set; }
            internal Stream Output { get; set; }
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
