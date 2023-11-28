using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using XProtectWebStream.Database;
using XProtectWebStream.Global;
using XProtectWebStream.Milestone;
using XProtectWebStream.Shared;

namespace XProtectWebStream.Web
{
    internal class WebServer
    {
        private readonly HttpListener httpListener;
        private bool running = false;

        public event EventHandler<Exception> OnError;
        public event EventHandler<string> OnLog;

        private readonly List<WebSocketClient> webSocketClients;
        private readonly MilestoneConnection milestoneConnection;
        private readonly TokenManager tokenManager;
        private readonly WebVideoManager webVideoManager;
        private readonly LinkSender linkSender;
        private readonly WebSessionsManager sessionsManager;
        private readonly LocalDatabase database;
        private readonly WebApi webApi;

        internal WebServer()
        {
            httpListener = new HttpListener();
            var http = Config.Instance.UseHttps ? "https" : "http";
            httpListener.Prefixes.Add($"{http}://{Config.Instance.Binding}:{Config.Instance.Port}/");

            database = new LocalDatabase();
            database.OnLog += LocalDb_OnLog;

            linkSender = new LinkSender(Config.Instance.LicenseKey, "TryggSTREAM", "TryggS");

            tokenManager = new TokenManager(database);
            tokenManager.OnLog += Type_OnLog;

            sessionsManager = new WebSessionsManager();
            webSocketClients = new List<WebSocketClient>();
            milestoneConnection = new MilestoneConnection(tokenManager, linkSender, database);
            milestoneConnection.OnLog += Type_OnLog;

            webApi = new WebApi(tokenManager, database);
            webVideoManager = new WebVideoManager(tokenManager, database.AccessDatabaseManagement);
        }

        internal void Start()
        {
            milestoneConnection.RegisterEndpoints();

            httpListener.Start();
            running = true;

            Log("Starting webserver on " + string.Join(", ", httpListener.Prefixes.Cast<string>()));

            new Thread(ListenThread) { Name = "Http listen thread" }.Start();
        }

        private void LocalDb_OnLog(object sender, string e)
        {
            Log("Database: " + e);
        }

        internal void Close()
        {
            webSocketClients.ForEach(wsc => wsc.Close());

            httpListener.Close();
            running = false;
            milestoneConnection.Close();
            tokenManager.Close();
            database.Close();
        }

        void ListenThread()
        {
            while (running)
            {
                try
                {
                    HttpListenerContext context = httpListener.GetContext();

                    ThreadPool.QueueUserWorkItem((state) =>
                    {
                        NewContext(context);
                    });

                }
                catch (Exception e)
                {
                    OnError?.Invoke(this, e);
                }
            }
        }

        void NewContext(object context)
        {
            HttpListenerContext ctx = context as HttpListenerContext;

            HttpListenerRequest request = ctx.Request;

            Cookie clientIdCookie = request.Cookies["sessionId"];
            string sessionId = clientIdCookie?.Value;
            WebSession webSession = null;

            if (sessionId != null)
            {
                webSession = sessionsManager.GetSession(sessionId);
            }

            if (webSession == null)
                webSession = sessionsManager.NewSession(TimeSpan.FromMinutes(15));
            else
                webSession.RenewSessionId();

            webSession.RemoteEndpoint = ctx.Request.RemoteEndPoint;

            if(sessionId != webSession.SessionId)
            {
                clientIdCookie = new Cookie("sessionId", webSession.SessionId);
                clientIdCookie.Expires = DateTime.Now.AddYears(1);
                ctx.Response.AppendCookie(clientIdCookie);
            }

            var userAgent = request.Headers["user-agent"];

            if (request.IsWebSocketRequest && request.Url.Segments[1] == "ws")
            {
                HandleWebSocket(ctx, webSession);
            }
            else if(request.IsWebSocketRequest)
            {
                ctx.Response.StatusCode = 500;
                ctx.Response.Close();
            }
            else if (request.Url.Segments.Length > 1)
            {
                var requestedResource = request.Url.Segments[1];
                if (requestedResource == "api")
                {
                    webApi.HandleApiRequest(ctx, webSession);
                }
                else if(requestedResource == "video")
                {
                    var requestQuery = System.Web.HttpUtility.ParseQueryString(request.Url.Query);
                    string token = null;

                    if(requestQuery.AllKeys.Contains("t"))
                        token = requestQuery["t"].Trim();
                    
                    if (string.IsNullOrEmpty(token))
                    {
                        ctx.Response.StatusCode = 404;
                        ctx.Response.OutputStream.Close();
                    }
                    else
                    {
                        webVideoManager.HandleRequest(ctx, webSession, token);
                    }
                }
                //else if(requestedResource == "log" || requestedResource == "log.html")
                //{
                //    if(webSession.RemoteEndpoint.IsLocalMachine()) // Only works locally!
                //        WebFile.HandleRequest(ctx, "log.html");
                //    else
                //    {
                //        ctx.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                //        ctx.Response.Close();
                //    }
                //}
                else
                {
                    WebFile.HandleRequest(ctx);
                }
            }
            else if(IsInternetExplorer(userAgent))
            {
                WebFile.HandleRequest(ctx, "unsupported.html");
            }
            else
            {
                WebFile.HandleRequest(ctx, "index.html");
            }
        }

        async void HandleWebSocket(HttpListenerContext ctx, WebSession webSession)
        {
            WebSocketContext webSocketContext = null;
            string token = null;

            try
            {
                webSocketContext = await ctx.AcceptWebSocketAsync(subProtocol: null);
                var webSocketQuery = System.Web.HttpUtility.ParseQueryString(webSocketContext.RequestUri.Query);

                token = webSocketQuery["t"].Trim();

                if (token == null)
                    throw new Exception("Missing 't' query");

            }
            catch (Exception e)
            {
                // The upgrade process failed somehow.For simplicity lets assume it was a failure on the part of the server and indicate this using 500.
                ctx.Response.StatusCode = 500;
                ctx.Response.Close();
                Log("Exception: " + e.Message);
            }

            Log("Client connected with t = " + token);

            var cameraToken = tokenManager.GetCameraToken(token);

            if (cameraToken == null)
            {
                await webSocketContext.WebSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Invalid token", CancellationToken.None);

                //ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                ctx.Response.Close();
                Log("Invalid token (" + token + ")");
            }
            else if ((cameraToken.Authentication == CameraToken.AuthType.Password && webSession.HaveAccessTo(cameraToken) == false) || 
                (cameraToken.Authentication == CameraToken.AuthType.BankID && (webSession.BankIDLogin?.Data == null || database.AccessDatabaseManagement.UserHaveAccessTo(cameraToken.AccessGroupIds, webSession.BankIDLogin.Data.user.personalNumber) == false))) // No access
            {
                await webSocketContext.WebSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Not authenticated", CancellationToken.None);
                ctx.Response.Close();
                Log("No pw for " + token);
            }
            else if(cameraToken.VideoType != CameraToken.TokenVideoType.Live)
            {
                await webSocketContext.WebSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Wrong video type", CancellationToken.None);
                ctx.Response.Close();
                Log("Wrong video type (not live) for " + token);
            }
            else
            {
                var imageProvider = milestoneConnection.GetImageProvider(cameraToken.CameraId);

                if (imageProvider == null)
                    throw new Exception("Image provider was null???");

                var client = new WebSocketClient(webSocketContext, webSession, cameraToken, imageProvider, database.AccessDatabaseManagement);
                client.OnLog += Type_OnLog;
                client.OnClose += Client_OnClose;
                webSocketClients.Add(client);
            }
        }

        private void Type_OnLog(object sender, string e)
        {
            Log(sender.GetType().Name + ": " + e);
        }

        private void Client_OnClose(object sender, EventArgs e)
        {
            var client = sender as WebSocketClient;
            webSocketClients.Remove(client);
            client = null;
        }

        private void Log(string msg)
        {
            OnLog?.Invoke(this, msg);
        }


        private static bool IsInternetExplorer(string userAgent)
        {
            if (userAgent == null)
                return false;

            return Regex.IsMatch(userAgent, @"MSIE|Internet Explorer") || Regex.IsMatch(userAgent, @"Trident/7.0(.*)?; rv:11.0");
            //return preg_match ('~MSIE|Internet Explorer~i', $_SERVER['HTTP_USER_AGENT']) || preg_match('~Trident/7.0(.*)?; rv:11.0~', $_SERVER['HTTP_USER_AGENT']);
        }
    }
}
