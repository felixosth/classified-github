using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using XProtectWebStream.Global;

namespace XProtectWebStream.Web
{
    internal class WebVideoManager
    {
        private TokenManager tokenManager;
        Database.AccessDatabaseManagement accessDatabase;

        internal WebVideoManager(TokenManager tokenManager, Database.AccessDatabaseManagement accessDatabase)
        {
            this.tokenManager = tokenManager;
            this.accessDatabase = accessDatabase;
        }

        internal void HandleRequest(HttpListenerContext ctx, WebSession webSession, string token)
        {
            var cameraToken = tokenManager.GetCameraToken(token);

            if(cameraToken == null ||
                cameraToken.VideoType != CameraToken.TokenVideoType.Recorded ||
                cameraToken.HaveExpired)
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                ctx.Response.OutputStream.Close();
                return;
            }
            else if((cameraToken.Authentication == CameraToken.AuthType.Password && webSession.HaveAccessTo(cameraToken) == false) || 
                (cameraToken.Authentication == CameraToken.AuthType.BankID && (webSession.BankIDLogin?.Data == null || accessDatabase.UserHaveAccessTo(cameraToken.AccessGroupIds, webSession.BankIDLogin.Data.user.personalNumber) == false))) // No access
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                ctx.Response.OutputStream.Close();
                return;
            }
            else
            {
                WebFile.HandleRequest(ctx, cameraToken.VideoFile, etag: token, fileType: "mp4");
            }
        }
    }
}
