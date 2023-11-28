using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using XProtectWebStream.Database;
using XProtectWebStream.Database.Objects;
using XProtectWebStream.Global;
using XProtectWebStream.Shared;

namespace XProtectWebStream.Web
{
    internal class WebApi
    {
        bool useGzip = false;

        TokenManager tokenManager;

        LocalDatabase database;

        internal WebApi(TokenManager tokenManager, LocalDatabase database)
        {
            this.tokenManager = tokenManager;
            this.database = database;
        }

        internal void HandleApiRequest(HttpListenerContext ctx, WebSession webSession)
        {
            var response = ctx.Response;
            var request = ctx.Request;

            response.ContentEncoding = Encoding.UTF8;

            if (request.Headers.AllKeys.Contains("Accept-Encoding"))
            {
                useGzip = request.Headers["Accept-Encoding"].ToLower().Contains("gzip");
                //response.Headers.Add("Content-Encoding", "gzip");
            }

            switch (request.HttpMethod.ToUpper())
            {
                case "GET":
                    switch (request.QueryString["action"])
                    {
                        case "activateToken":

                            CameraToken cameraToken = null;
                            string t = request.QueryString["token"];

                            if(t != null)
                                cameraToken = tokenManager.GetCameraToken(t);

                            if (cameraToken == null) // Token not found
                            {
                                ReturnWrite(new { token = false }, ctx, useGzip);
                            }
                            else
                            {
                                if(cameraToken.Authentication == CameraToken.AuthType.BankID && webSession.HaveAccessTo(cameraToken) == false) // Require BankID
                                {
                                    if(webSession.BankIDLogin?.Data != null) // Do we have the clients data?
                                    {
                                        if (database.AccessDatabaseManagement.UserHaveAccessTo(cameraToken.AccessGroupIds, webSession.BankIDLogin.Data.user.personalNumber)) // Does the client have access?
                                        {
                                            database.InsertWebAccess(DateTime.Now, webSession.RemoteEndpoint.ToString(), webSession.SessionId, cameraToken.Token);
                                            cameraToken.ActivateToken();
                                            ReturnWrite(new { token = true, expires = cameraToken.GetRelativeExpirationDate(), type = cameraToken.VideoType, createdBy = cameraToken.CreatedBy, name = cameraToken.CameraName, comment = cameraToken.Comment }, ctx, useGzip);
                                        }
                                        else
                                        {
                                            ReturnWrite(new { token = true, bankId = true, unauthorized = true }, ctx, useGzip); // The client doesnt have access
                                        }
                                    }
                                    else // Create new bankid login
                                    {
                                        if(webSession.BankIDLogin == null || webSession.BankIDLogin.LoginExpired || webSession.BankIDLogin.LastStatus != Global.Auth.BankIDResponses.Status.pending)
                                            webSession.BankIDLogin = new Global.Auth.BankIDLogin(); // Create order

                                        ReturnWrite(new { token = true, 
                                            bankId = true, autoStartToken = webSession.BankIDLogin.AutoStartToken, qrdata = webSession.BankIDLogin.Order.GetAnimQRData() },
                                            ctx, useGzip); // Tell the client to auth with bankid, the interaction will continue with bankIdCollect
                                    }
                                }
                                else if(cameraToken.Authentication == CameraToken.AuthType.Password && webSession.HaveAccessTo(cameraToken) == false) // Require password, the interaction will continue with POST
                                {
                                    ReturnWrite(new { token = true, passwordRequred = true }, ctx, useGzip); // Tell the client to supply a password
                                }
                                else
                                {
                                    // Password accepted
                                    database.InsertWebAccess(DateTime.Now, webSession.RemoteEndpoint.ToString(), webSession.SessionId, cameraToken.Token);
                                    cameraToken.ActivateToken();
                                    ReturnWrite(new { token = true, expires = cameraToken.GetRelativeExpirationDate(), type = cameraToken.VideoType, createdBy = cameraToken.CreatedBy, name = cameraToken.CameraName, comment = cameraToken.Comment }, ctx, useGzip);
                                }
                            }
                            break;

                        case "bankIdCollect":

                            if(webSession.BankIDLogin == null)
                            {
                                response.StatusCode = (int)HttpStatusCode.BadRequest;
                            }
                            else if(webSession.BankIDLogin.LastStatus != Global.Auth.BankIDResponses.Status.pending)
                            {
                                ReturnWrite(new { token = true, bankId = true, status = "failed" }, ctx, useGzip);
                            }
                            else
                            {
                                var status = webSession.BankIDLogin.Collect();
                                if(status == Global.Auth.BankIDResponses.Status.complete && webSession.BankIDLogin.Data != null)
                                {
                                    database.InsertBankIDLogin(DateTime.Now, webSession.SessionId, webSession.BankIDLogin.Data.user.personalNumber);
                                }
                                ReturnWrite(new { token = true, bankId = true, status = status.ToString(), qrdata = webSession.BankIDLogin.Order.GetAnimQRData() }, ctx, useGzip);
                            }

                            break;

                        case "getLog":
                            if(string.IsNullOrWhiteSpace(request.QueryString["log"]) == false && webSession.RemoteEndpoint.IsLocalMachine())
                            {
                                var logEntries = database.DynamicSelect(request.QueryString["log"]);

                                ReturnWrite(logEntries, ctx, useGzip);
                            }
                            break;

                        case "admin":

                            if(ctx.Request.RemoteEndPoint.IsLocalMachine() || Config.Instance.AllowAdminFromIPs.Any(ipp => ipp.IsInRange(ctx.Request.RemoteEndPoint.Address)))
                            {
                                switch(request.QueryString["adminAction"])
                                {
                                    case "getAccessGroups":
                                        ReturnWrite(database.AccessDatabaseManagement.GetAccessGroups(includeUnassigned: true), ctx, useGzip);
                                        break;
                                    //case "getAccessUsers":
                                    //    ReturnWrite(database.AccessDatabaseManagement.GetAccessUsers(), ctx, useGzip);
                                    //    break;

                                    default:
                                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                                        break;
                                }
                            }
                            else
                            {
                                response.StatusCode = (int)HttpStatusCode.Forbidden;
                            }

                            break;

                        default:
                            response.StatusCode = (int)HttpStatusCode.BadRequest;
                            break;
                    }
                break;

                case "POST":

                    using (var inputStream = request.InputStream)
                    using (var streamReader = new StreamReader(inputStream))
                    {
                        try
                        {
                            var inputString = streamReader.ReadToEnd();
                            //var inputDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(inputString);
                            dynamic input = JsonConvert.DeserializeObject<ExpandoObject>(inputString, new ExpandoObjectConverter());


                            switch(input.action)
                            {
                                case "activateToken":
                                    string inputPw = Encoding.ASCII.GetString(System.Convert.FromBase64String(input.password));
                                    CameraToken cameraToken = tokenManager.GetCameraToken(input.token);

                                    if(cameraToken != null)
                                    {
                                        if(cameraToken.Authentication == CameraToken.AuthType.Password && cameraToken.Password != inputPw) // Require password and wrong password
                                        {
                                            ReturnWrite(new { token = true, passwordRequred = true, wrongPassword = true }, ctx, useGzip);
                                        }
                                        else if(cameraToken.Authentication == CameraToken.AuthType.None || /* No auth required */
                                            (cameraToken.Authentication == CameraToken.AuthType.Password && cameraToken.Password == inputPw)) // Require password and right password supplied
                                        {
                                            webSession.AddAccessToToken(cameraToken);
                                            cameraToken.ActivateToken();
                                            database.InsertWebAccess(DateTime.Now, webSession.RemoteEndpoint.ToString(), webSession.SessionId, cameraToken.Token);
                                            ReturnWrite(new { token = true, expires = cameraToken.GetRelativeExpirationDate(), type = cameraToken.VideoType, createdBy = cameraToken.CreatedBy, name = cameraToken.CameraName, comment = cameraToken.Comment }, ctx, useGzip);
                                        }
                                    }
                                    else
                                    {
                                        // Token not found
                                        ReturnWrite(new { token = false }, ctx, useGzip);
                                    }
                                    break;

                                case "admin":
                                    if (ctx.Request.RemoteEndPoint.IsLocalMachine() || Config.Instance.AllowAdminFromIPs.Any(ipp => ipp.IsInRange(ctx.Request.RemoteEndPoint.Address)))
                                    {
                                        switch (input.adminAction)
                                        {
                                            case "addAccessGroup":
                                                ReturnWrite(new { result = database.AccessDatabaseManagement.InsertAccessGroup(new Database.Objects.AccessGroup(-1, (string)input.group.name)) }, ctx, useGzip);
                                                break;
                                            case "modifyAccessGroup":
                                                ReturnWrite(new { result = database.AccessDatabaseManagement.ModifyAccessGroup(new Database.Objects.AccessGroup((int)input.group.id, (string)input.group.name)) }, ctx, useGzip);
                                                break;
                                            case "deleteAccessGroup":
                                                ReturnWrite(new { result = database.AccessDatabaseManagement.DeleteAccessGroup((int)input.group.id) }, ctx, useGzip);
                                                break;

                                            case "addAccessUser":
                                                ReturnWrite(new { result = database.AccessDatabaseManagement.InsertAccessUser(new Database.Objects.AccessUser(-1, (int)input.user.groupId, (string)input.user.name, (string)input.user.pnr, hidePnr: false))}, ctx, useGzip);
                                                break;
                                            case "modifyAccessUser":
                                                ReturnWrite(new { result = database.AccessDatabaseManagement.ModifyAccessUser(new Database.Objects.AccessUser((int)input.user.id, (int)input.user.groupId, (string)input.user.name, null)) }, ctx, useGzip);
                                                break;
                                            case "deleteAccessUser":
                                                ReturnWrite(new { result = database.AccessDatabaseManagement.DeleteAccessUser((int)input.user.id) }, ctx, useGzip);
                                                break;

                                            default:
                                                response.StatusCode = (int)HttpStatusCode.BadRequest;
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        response.StatusCode = (int)HttpStatusCode.Forbidden;
                                    }
                                    break;
                                default:
                                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                                    break;
                            }
                        }
                        catch(Exception ex)
                        {
                            response.StatusCode = (int)HttpStatusCode.BadRequest;
                            ReturnWrite(new { error = ex.Message }, ctx, useGzip);
                        }
                    }

                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
            }

            response.OutputStream.Close();
        }

        private static string StreamToText(Stream stream)
        {
            using (StreamReader sr = new StreamReader(stream))
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
            try
            {
                ReturnWrite(JsonConvert.SerializeObject(objectToSerialize), ctx, useGzip);
            }
            catch
            {
                // todo throw or what?
            }
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

            if (useGzip)
            {
                ctx.Response.Headers.Add("Content-Encoding", "gzip");
            }

            ctx.Response.ContentType = "application/json";
            ctx.Response.ContentLength64 = buffer.Length;
            ctx.Response.OutputStream.Write(buffer, 0, buffer.Length);
        }
    }
}
