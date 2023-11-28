using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using XProtectWebStream.Global;
using XProtectWebStream.Shared;

namespace XProtectWebStream.Web
{
    internal static class WebFile
    {
        internal static void HandleRequest(HttpListenerContext ctx, string requestedFileOverride = null, string etag = null, string fileType = null)
        {
            var request = ctx.Request;
            var response = ctx.Response;

            //response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate";
            //response.Headers["Pragma"] = "no-cache";
            response.Headers["Date"] = UTCDate(DateTime.UtcNow);
            //response.Headers["Expires"] = UTCDate(DateTime.Now.AddSeconds(600).ToUniversalTime());
            //response.Headers["Age"] = "0";



            bool useGzip = false;
            if (request.Headers.AllKeys.Contains("Accept-Encoding"))
            {
                useGzip = request.Headers["Accept-Encoding"].ToLower().Contains("gzip");
                if (useGzip)
                    response.Headers.Add("Content-Encoding", "gzip");
            }

            var requestedFile = requestedFileOverride != null ? requestedFileOverride : request.Url.AbsolutePath;
            //var requestedFile = ctx.Request.RawUrl.Substring(1, ctx.Request.RawUrl.Length - 1);


            var filePath = !File.Exists(requestedFile) ? Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/Web/" + requestedFile : requestedFile;

            fileType = fileType == null ? Path.GetExtension(filePath) : fileType;


            System.IO.Stream output = response.OutputStream;
            byte[] fileBytes = null;

            if (!File.Exists(filePath))
            {
                response.StatusCode = 404;
                output.Close();
                return;
            }
            else if(new DirectoryInfo(Path.GetDirectoryName(filePath)).Name.ToLower() == "admin" && ctx.Request.RemoteEndPoint.IsLocalMachine() == false && Config.Instance.AllowAdminFromIPs.Any(ipp => ipp.IsInRange(ctx.Request.RemoteEndPoint.Address)) == false)
            {
                response.StatusCode = (int)HttpStatusCode.Forbidden;
                output.Close();
                return;
            }

            var fileInfo = new FileInfo(filePath);

            response.Headers["last-modified"] = UTCDate(fileInfo.LastWriteTime.ToUniversalTime());

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

            switch (fileType.Replace(".", ""))
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
                case "mp4":
                    response.Headers["Accept-Ranges"] = "0-" + fileBytes.Length;
                    response.ContentType = "video/mp4";
                    break;
                case "webm":
                    response.Headers["Accept-Ranges"] = "0-" + fileBytes.Length;
                    response.ContentType = "video/webm";
                    break;
                case "jpg":
                case "jpeg":
                    response.ContentType = "image/jpeg";
                    break;
                case "png":
                    response.ContentType = "image/png";
                    break;
                case "ico":
                    response.ContentType = "image/x-icon";
                    break;
                case "svg":
                    response.ContentType = "image/svg+xml";
                    break;
                default:
                    response.ContentType = "text/plain;";
                    break;
            }

            switch (fileType.Replace(".", ""))
            {
                case "html":
                case "css":
                case "js":
                case "jpg":
                case "jpeg":
                case "png":
                    response.Headers["Cache-Control"] = "max-age=86400";
                    break;
            }

            response.Headers["ETag"] = etag ?? Shared.Utils.CreateMD5(fileBytes);

            bool useBytesRange = false;
            int rangeBytesFrom = 0;
            int rangeBytesTo = -1;
            if (request.Headers.AllKeys.Contains("Range"))
            {
                useBytesRange = true;

                var rangeHeader = request.Headers["Range"].ToLower().Replace("bytes=", "");
                var rangeSplit = rangeHeader.Split('-');

                var from = rangeSplit[0];
                var to = rangeSplit[1];

                int.TryParse(from, out rangeBytesFrom);
                int.TryParse(to, out rangeBytesTo);

                if (from == "")
                {
                    rangeBytesFrom = fileBytes.Length - rangeBytesTo;
                    rangeBytesTo = fileBytes.Length - 1;
                }
                else if (to == "" || rangeBytesTo > fileBytes.Length - 1)
                {
                    rangeBytesTo = fileBytes.Length - 1;
                }

                if (rangeBytesFrom > rangeBytesTo)
                {
                    response.StatusCode = (int)HttpStatusCode.RequestedRangeNotSatisfiable;
                    response.Headers["Content-Range"] = "*/" + (fileBytes.Length);
                    output.Close();
                    return;
                }
            }

            try
            {
                if (useBytesRange)
                {
                    int bytesToWrite = rangeBytesTo - rangeBytesFrom;
                    response.ContentLength64 = bytesToWrite + 1;
                    response.Headers["Content-Range"] = $"bytes {rangeBytesFrom}-{rangeBytesTo}/{fileBytes.Length}";
                    //response.Headers["Accept-Ranges"] = "0-" + fileBytes.Length;

                    response.StatusCode = (int)HttpStatusCode.PartialContent;

                    output.Write(fileBytes, rangeBytesFrom, bytesToWrite + 1);
                }
                else
                {
                    response.ContentLength64 = fileBytes.Length;
                    //response.Headers["Accept-Ranges"] = "0-" + fileBytes.Length;

                    output.Write(fileBytes, 0, fileBytes.Length);
                }
            }
            catch /*(Exception ex)*/
            {

                //throw;
            }
            finally
            {
                try
                {
                    output.Flush();
                    output.Close();
                }
                catch
                {

                }
            }

        }

        private static string UTCDate(DateTime date) => date.ToString("ddd, dd MMM yyyy HH:mm:ss K", CultureInfo.InvariantCulture);
    }
}
