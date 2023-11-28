using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XProtectWebStream.Global;
using XProtectWebStream.Milestone;
using XProtectWebStream.Database;

namespace XProtectWebStream.Web
{
    internal class WebSocketClient
    {
        private readonly WebSocket webSocket;
        private readonly MilestoneImageProvider imageProvider;
        private readonly CameraToken cameraToken;
        private readonly WebSession webSession;
        private readonly AccessDatabaseManagement accessDatabaseManagement;

        internal event EventHandler OnClose;
        internal event EventHandler<string> OnLog;

        public WebSocketClient(WebSocketContext wsctx, WebSession webSession, CameraToken cameraToken, MilestoneImageProvider imageProvider, AccessDatabaseManagement accessDatabaseManagement)
        {
            this.imageProvider = imageProvider;
            this.webSocket = wsctx.WebSocket;
            this.cameraToken = cameraToken;
            this.webSession = webSession;
            this.accessDatabaseManagement = accessDatabaseManagement;

            cameraToken.TokenExpired += CameraToken_TokenExpired;

            cameraToken.ActivateToken();

            ThreadPool.QueueUserWorkItem((state) => StreamData());
        }

        private void CameraToken_TokenExpired(object sender, EventArgs e)
        {
            CloseConnection();
        }

        private async void StreamData()
        {
            try
            {
                int lastImageHash = 0;
                byte[] receiveBuffer;

                while (webSocket.State == WebSocketState.Open && !cameraToken.HaveExpired)
                {
                    if(HaveBankIDRights() == false)
                    {
                        CloseConnection("Rights revoked");
                        break;
                    }

                    var newImageHash = imageProvider.ImageHash;

                    if(newImageHash != lastImageHash)
                    {
                        lastImageHash = newImageHash;
                        receiveBuffer = imageProvider.ImageBytes.ToArray(); // Clone

                        await webSocket.SendAsync(new ArraySegment<byte>(receiveBuffer), WebSocketMessageType.Binary, true, CancellationToken.None);
                    }
                    else
                    {
                        Thread.Sleep(33);
                    }
                }
            }
            catch(Exception ex)
            {
                OnLog?.Invoke(this, ex.Message);
                CloseConnection("Error");
            }
        }

        private bool HaveBankIDRights()
        {
            if (cameraToken.Authentication != CameraToken.AuthType.BankID)
                return true;

            return accessDatabaseManagement.UserHaveAccessTo(cameraToken.AccessGroupIds, webSession.BankIDLogin?.Data?.user.personalNumber);
        }

        public void Close()
        {
            CloseConnection("Shutting down application");
        }

        private async void CloseConnection(string reason = "Token expired")
        {
            try
            {
                if (webSocket != null && webSocket.State == WebSocketState.Open)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, reason, CancellationToken.None);
                }
            }
            catch
            {
            }

            if(cameraToken != null)
                cameraToken.TokenExpired -= CameraToken_TokenExpired;

            OnClose?.Invoke(this, new EventArgs());
            OnLog?.Invoke(this, "Connection closed: " + reason);
        }
    }
}
