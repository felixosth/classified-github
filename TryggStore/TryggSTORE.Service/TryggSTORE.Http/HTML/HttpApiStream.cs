using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TryggSTORE.Http.HTML
{
    internal class HttpApiStream
    {
        bool useGzip = false;
        private TryggSTOREServer httpServer;
        public HttpApiStream(TryggSTOREServer server)
        {
            httpServer = server;
        }

        internal void HandleApiRequest(HttpListenerContext ctx)
        {
            var response = ctx.Response;
            var request = ctx.Request;

            response.ContentEncoding = Encoding.UTF8;
            if (request.Headers.AllKeys.Contains("Accept-Encoding"))
            {
                useGzip = request.Headers["Accept-Encoding"].ToLower().Contains("gzip");
                //response.Headers.Add("Content-Encoding", "gzip");
            }

            if (request.QueryString.AllKeys.Any(k => k == "action"))
            {
                switch(request.HttpMethod.ToUpper())
                {
                    case "GET":
                        switch (request.QueryString["action"])
                        {
                            case "getData":
                                var configJson = JsonConvert.SerializeObject(ConfigFile.Instance);
                                ReturnWrite(configJson, ctx, useGzip);
                                break;

                            case "getNetwork":


                                string networkDataString = null;
                                lock (httpServer.ActiveMJPEGStreams)
                                {
                                    var eh = httpServer.ActiveMJPEGStreams.Select(stream => new { stream.Client, stream.KbpsBandwidth });
                                    networkDataString = JsonConvert.SerializeObject(eh);
                                }
                                ReturnWrite(networkDataString, ctx, useGzip);

                                break;

                            case "stop":
                                if(request.QueryString.AllKeys.Any(k => k == "value"))
                                {
                                    string stopValueText = request.QueryString["value"]; // we expect bool
                                    bool stopValue = false;
                                    if(bool.TryParse(stopValueText, out stopValue))
                                    {
                                        ConfigFile.Instance.EmergencyStop = stopValue;
                                        ReturnWrite(new { success = true }, ctx, useGzip);
                                    }
                                }
                                else
                                {
                                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                                }
                                break;
                        }
                        break;
                    case "POST":
                        switch (request.QueryString["action"])
                        {
                            case "setData":
                                var inputConfigText = StreamToText(request.InputStream);
                                try
                                {
                                    var newConfigFile = JsonConvert.DeserializeObject<ConfigFile>(inputConfigText);
                                    ConfigFile.Instance.UpdateValues(newConfigFile);
                                    ConfigFile.Instance.SaveConfig();
                                    ReturnWrite(new{ success = true}, ctx, useGzip);
                                }
                                catch(Exception ex)
                                {
                                    ReturnWrite(new { success = false, error = ex.Message }, ctx, useGzip);
                                }
                                break;
                        }
                        break;
                }
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            response.OutputStream.Close();
        }

        private static string StreamToText(Stream stream)
        {
            using(StreamReader sr = new StreamReader(stream))
            {
                return sr.ReadToEnd();
            }
        }

        private static byte[] TextToBytes(string text)
        {
            return Encoding.UTF8.GetBytes(text);
        }

        private static void ReturnWrite(object objectToSerialize, HttpListenerContext ctx, bool useGzip)
        {
            ReturnWrite(JsonConvert.SerializeObject(objectToSerialize), ctx, useGzip);
        }

        private static void ReturnWrite(string text, HttpListenerContext ctx, bool useGzip)
        {
            byte[] textBuffer = TextToBytes(text);
            byte[] buffer = null;

            if (useGzip)
            {
                using (var ms = new MemoryStream())
                {
                    if (useGzip)
                    {
                        using (var gzip = new GZipStream(ms, CompressionMode.Compress))
                            gzip.Write(textBuffer, 0, textBuffer.Length);
                    }
                    buffer = ms.ToArray();
                }
            }
            else
                buffer = textBuffer;

            if(useGzip)
            {
                ctx.Response.Headers.Add("Content-Encoding", "gzip");
            }

            ctx.Response.ContentType = "application/json";
            ctx.Response.ContentLength64 = buffer.Length;
            ctx.Response.OutputStream.Write(buffer, 0, buffer.Length);
        }
    }
}
