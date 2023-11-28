using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TryggSTORE.Http.HTML
{
    internal static class HttpFileStream
    {
        internal static void HandleRequest(HttpListenerContext ctx)
        {
            var request = ctx.Request;
            var response = ctx.Response;

            bool useGzip = false;
            if(request.Headers.AllKeys.Contains("Accept-Encoding"))
            {
                useGzip = request.Headers["Accept-Encoding"].ToLower().Contains("gzip");
                response.Headers.Add("Content-Encoding", "gzip");
            }

            var requestedFile = ctx.Request.RawUrl.Substring(1, ctx.Request.RawUrl.Length-1);


            var filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/Resources/Web/" + requestedFile;

            var fileType = Path.GetExtension(filePath);

            System.IO.Stream output = response.OutputStream;
            byte[] fileBytes = null;
            if (File.Exists(filePath))
            {
                using (var ms = new MemoryStream())
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    if (useGzip)
                        using (var gzip = new GZipStream(ms, CompressionMode.Compress))
                            fs.CopyTo(gzip);
                    else
                        fs.CopyTo(ms);

                    fileBytes = ms.ToArray();
                }
            }
            else
            {
                response.StatusCode = 404;
                output.Close();
                return;
            }

            switch(fileType.Replace(".", ""))
            {
                case "html":
                    response.ContentType = "text/html;";
                    break;
                case "css":
                    response.ContentType = "text/css;";
                    break;
                case "js":
                    response.ContentType = "application/javascript";
                    break;
                default:
                    response.ContentType = "text/plain;";
                    break;
            }


            response.ContentLength64 = fileBytes.Length;
            
            output.Write(fileBytes, 0, fileBytes.Length);

            output.Close();
        }
    }
}
