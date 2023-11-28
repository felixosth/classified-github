using System;
using System.Diagnostics;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

namespace WebcamBridge
{
    /// <summary>
    /// Self-hosted Web Server class
    /// https://codehosting.net/blog/BlogEngine/post/Simple-C-Web-Server
    /// </summary>
    public class WebServer
    {
        private readonly HttpListener _listener = new HttpListener();
        private readonly Action<HttpListenerContext> _responderMethod;
        private readonly Action<string> _logMethod;
        private string[] _pathsToListen;
        public bool LogOnEachRequest { get; set; } = false;
        public bool IsRunning { get; set; } = false;

        /// <summary>
        /// Creates an HTTP Listener
        /// </summary>
        /// Array of paths to listen to (e.g. 'http://localhost:8081/test1/','http://localhost:8081/test2/')
        /// A callback method which will be called on each request and which allows you to create a corresponding response
        /// A callback method for providing logs
        public WebServer(string[] pathsToListen, Action<HttpListenerContext> responseCallback, Action<string> logCallBack = null)
        {
            _logMethod = logCallBack;
            _pathsToListen = pathsToListen;

            if (!HttpListener.IsSupported)
                throw new NotSupportedException(
                    "Needs Windows XP SP2, Server 2003 or later.");

            // URI prefixes are required, for example 
            // "http://localhost:8080/index/".
            if (pathsToListen == null || pathsToListen.Length == 0)
                throw new ArgumentException("pathsToListen are missing!");

            // A responder method is required
            if (responseCallback == null)
                throw new ArgumentException("method is missing!");

            foreach (string s in pathsToListen)
            {
                string s1 = s;
                if (!s1.EndsWith("/"))
                    s1 = s1 + "/";
                _listener.Prefixes.Add(s1);
            }

            _responderMethod = responseCallback;
        }

        /// <summary>
        /// Start listening
        /// </summary>
        public void Start()
        {
            _listener.Start();
            IsRunning = true;
            ThreadPool.QueueUserWorkItem((o) =>
            {
                log("WebServer - started, listening on " + String.Join(" , ", _pathsToListen));
                try
                {
                    while (_listener.IsListening)
                    {
                        ThreadPool.QueueUserWorkItem((c) =>
                        {
                            var ctx = c as HttpListenerContext;

                            try
                            {
                                if (LogOnEachRequest)
                                    log(String.Format("WebServer - request from {2}: {0} {1}", ctx.Request.HttpMethod, ctx.Request.Url, ctx.Request.LocalEndPoint.ToString()));
                                _responderMethod(ctx);
                            }
                            catch(ObjectDisposedException)
                            {
                                log("WebServer - Connection closed");
                            }
                            catch (Exception ex)
                            {
                                log("WebServer exception: " + ex);
                            }
                            finally
                            {
                                // always close the stream
                                ctx.Response.OutputStream.Close();
                            }
                        }, _listener.GetContext());
                    }
                }
                catch (Exception ex)
                {
                    log("WebServer exception: " + ex);
                }
            });
        }

        /// <summary>
        /// Stop listening
        /// </summary>
        public void Stop()
        {
            if (_listener.IsListening)
            {
                _listener.Stop();
                _listener.Close();
                log("WebServer stopped.");
            }
        }

        /// <summary>
        /// Just a sample response callback which writes some text to the output stream.
        /// You should provide your own response callback method in the constructor.
        /// </summary>
        /// 
        public static void BuiltInResponseCallback(HttpListenerContext ctx)
        {
            // Is it request with Body?
            String body = "";
            if (ctx.Request.HasEntityBody)
            {
                using (System.IO.Stream bodyStream = ctx.Request.InputStream) // here we have data
                {
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(bodyStream, ctx.Request.ContentEncoding))
                    {
                        body = reader.ReadToEnd();
                    }
                }
            }

            String responseString = string.Format("WebServer BuiltInResponseCallback<br>Time: {0}<br>Request:{1}<br>Body: {2}", DateTime.Now, ctx.Request.Url, body);
            byte[] buf = Encoding.UTF8.GetBytes(responseString);
            ctx.Response.ContentLength64 = buf.Length;
            ctx.Response.OutputStream.Write(buf, 0, buf.Length);
            ctx.Response.ContentType = "text/html";
        }

        /// <summary>
        /// Log callback
        /// </summary>
        /// 
        void log(string msg)
        {
            if (_logMethod != null)
            {
                _logMethod(msg);
            }
        }
    }
}