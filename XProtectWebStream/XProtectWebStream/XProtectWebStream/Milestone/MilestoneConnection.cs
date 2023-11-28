using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VideoOS.Platform;
using VideoOS.Platform.Messaging;
using XProtectWebStream.Database;
using XProtectWebStream.Database.Objects;
using XProtectWebStream.Global;
using XProtectWebStream.Shared;

namespace XProtectWebStream.Milestone
{
    internal class MilestoneConnection
    {
        private readonly Dictionary<string, MilestoneImageProvider> imageProviders = new Dictionary<string, MilestoneImageProvider>();

        private MessageCommunication msgCom;
        private List<object> messageObjects = new List<object>();
        TokenManager tokenManager;
        LinkSender linkSender;

        private bool licenseIsValid = true;

        MilestoneVideoExporter videoExporter;

        internal event EventHandler<string> OnLog;
        LocalDatabase database;

        MilestoneClientManager clientManager;

        internal MilestoneConnection(TokenManager tokenManager, LinkSender linkSender, LocalDatabase localDatabase)
        {
            this.tokenManager = tokenManager;
            this.linkSender = linkSender;
            this.database = localDatabase;

            database.AccessDatabaseManagement.AccessGroupsUpdated += AccessDatabaseManagement_AccessGroupsUpdated;

            LicenseSystem.OnLicenseValid += LicenseSystem_OnLicenseValid;
            LicenseSystem.OnLicenseExpired += LicenseSystem_OnLicenseExpired;

            VideoOS.Platform.SDK.Environment.Initialize();
            VideoOS.Platform.SDK.Media.Environment.Initialize();

            var server = new Uri(Config.Instance.MilestoneServer);
            VideoOS.Platform.SDK.Environment.AddServer(server, System.Net.CredentialCache.DefaultNetworkCredentials);
            VideoOS.Platform.SDK.Environment.Login(server, false);
            if (VideoOS.Platform.SDK.Environment.IsLoggedIn(server))
            {
                videoExporter = new MilestoneVideoExporter();
                videoExporter.OnLog += VideoExporter_OnLog;
                RefreshConfigOnInterval(TimeSpan.FromHours(1));
            }
            else
            {
                throw new Exception("not connected");
            }
        }

        internal void RegisterEndpoints()
        {
            MessageCommunicationManager.Start(EnvironmentManager.Instance.MasterSite.ServerId);
            msgCom = MessageCommunicationManager.Get(EnvironmentManager.Instance.MasterSite.ServerId);
            msgCom.ConnectionStateChangedEvent += MsgCom_ConnectionStateChangedEvent;
            clientManager = new MilestoneClientManager(msgCom);

            RegisterFilter(GlobalMessageCommunication, Constants.Messaging.GlobalMessageId);
            RegisterFilter(GlobalLinkSenderMessageCommunication, Constants.Messaging.GlobalLinkSenderMessageId);
            RegisterFilter(GlobalFeatureRequestMessageCommunication, Constants.Messaging.GlobalFeatureRequestMessageId);
            RegisterFilter(GlobalAccessGroupsRequestMessageCommunication, Constants.Messaging.GlobalAccessGroupsRequestMessageId);

            BroadcastFeatures(null);
            BroadcastAccessGroups(null);
        }

        internal void RegisterFilter(MessageReceiver messageReceiver, string messageId)
        {
            var comidFilter = new CommunicationIdFilter(messageId);
            var obj = msgCom.RegisterCommunicationFilter(messageReceiver, comidFilter);
            
            if(Config.Instance.WaitForMilestoneComReg)
                msgCom.WaitForCommunicationFilterRegistration(comidFilter);
            
            messageObjects.Add(obj);
        }

        private void LicenseSystem_OnLicenseExpired(object sender, EventArgs e)
        {
            if (licenseIsValid)
            {
                licenseIsValid = false;
                BroadcastFeatures(null);
            }

        }

        private void LicenseSystem_OnLicenseValid(object sender, EventArgs e)
        {
            if(!licenseIsValid)
            {
                licenseIsValid = true;
                BroadcastFeatures(null);
            }
        }

        private void MsgCom_ConnectionStateChangedEvent(object sender, EventArgs e)
        {
            Log("MsgCom state changed, IsConnected = " + msgCom.IsConnected);
        }

        private void VideoExporter_OnLog(object sender, string e)
        {
            Log("Exporter: " + e);
        }

        private void RefreshConfigOnInterval(TimeSpan interval)
        {
            new Thread(() =>
            {
                var wait = DateTime.Now.Add(interval);
                while(true)
                {
                    if(DateTime.Now > wait)
                    {
                        Configuration.Instance.RefreshConfiguration(Kind.Camera);
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
            })
            { IsBackground = true }.Start();
        }

        internal MilestoneImageProvider GetImageProvider(string cameraId)
        {
            var cameraGuid = Guid.Parse(cameraId);
            var camera = Configuration.Instance.GetItem(cameraGuid, Kind.Camera);

            cameraId = cameraGuid.ToString();

            if (camera != null)
            {
                if (!imageProviders.ContainsKey(cameraId))
                {
                    imageProviders[cameraId] = new MilestoneImageProvider(camera, cameraId);
                    imageProviders[cameraId].OnClose += ImageProvider_OnClose;
                    imageProviders[cameraId].OnLog += ImageProvider_OnLog;
                }

                return imageProviders[cameraId];
            }
            return null;
        }

        private void ImageProvider_OnLog(object sender, string e)
        {
            Log("ImageProvider: " + e);
        }

        private void ImageProvider_OnClose(object sender, EventArgs e)
        {
            imageProviders.Remove((sender as MilestoneImageProvider).CameraId);
        }

        protected static List<Item> GetAllItems(List<Item> items)
        {
            List<Item> result = new List<Item>();
            foreach (var item in items)
            {
                if (item.FQID.FolderType == FolderType.No)
                    result.Add(item);
                else
                    result.AddRange(GetAllItems(item.GetChildren()));
            }
            return result;
        }

        object GlobalLinkSenderMessageCommunication(Message message, FQID dest, FQID source)
        {
            try
            {
                var request = Utils.Packer.Deserialize<SendLinkRequest>((string)message.Data);
                if(request != null)
                {
                    ThreadPool.QueueUserWorkItem((state) =>
                    {
                        HandleLinkSenderRequest(request);
                    });
                }
            }
            catch(Exception ex)
            {
                Log(ex.Message);
            }

            return null;
        }

        private void HandleLinkSenderRequest(SendLinkRequest request)
        {
            var camToken = tokenManager.GetCameraToken(request.Token);

            if (camToken == null)
                return;

            switch(request.LinkType)
            {
                case SendLinkRequest.SendLinkType.Email:
                    linkSender.SendEmail(camToken.VideoType.ToString() + " camera web access",
                        "Hello, you've been granted " + camToken.VideoType.ToString().ToLower() + " web access to " + (camToken.CameraName ?? "a camera") + ".\r\n\r\n" + request.FullLink,
                        request.Recipient);
                    break;
                case SendLinkRequest.SendLinkType.SMS:
                    linkSender.SendSMS("Hello, you've been granted " + camToken.VideoType.ToString().ToLower() + " web access to " + (camToken.CameraName ?? "a camera") + ": " + request.FullLink,
                        request.Recipient);
                    break;
            }
        }

        object GlobalMessageCommunication(Message message, FQID dest, FQID source)
        {
            try
            {
                ThreadPool.QueueUserWorkItem((state) =>
                {
                    HandleResponse(message, source);
                });
            }
            catch(Exception ex)
            {
                Log(ex.Message);
            }
            
            return null;
        }

        private void HandleResponse(Message message, FQID source)
        {
            var payload = (string)message.Data;

            var request = Utils.Packer.Deserialize<TokenRequest>(payload);

            Log($"Incoming request for {request.CameraId} ({request.ReqType.ToString()})");

            TokenResponse response = null;
            CameraToken camToken = null;
            Item cam = null;

            var milestoneUsername = clientManager.GetClientId(message.ExternalMessageSourceEndPoint);


            if (request.ReqType != TokenRequest.RequestType.Revoke)
                cam = Configuration.Instance.GetItem(Guid.Parse(request.CameraId), Kind.Camera);

            if(licenseIsValid == false)
            {
                response = new TokenResponse() { Error = "License is expired or invalid!" };
            }
            else if(request.ReqType == TokenRequest.RequestType.Revoke)
            {
                // Todo: Rights to revoke?
                camToken = tokenManager.GetCameraToken(request.TokenToRevoke);

                if(camToken != null)
                    camToken.Revoke();

                response = new TokenResponse()
                {
                    IsRevoked = camToken != null,
                    Token = request.TokenToRevoke
                };
            }
            else if(cam == null)
            {
                response = new TokenResponse() { Error = "Camera not found" };
            }
            else if (request.ReqType == TokenRequest.RequestType.Live)
            {
                camToken = tokenManager.CreateNewLiveToken(request.CameraId, request.ExpireAfter, request.Password, request.Comment, request.RequireBankID, request.AccessGroups);
                camToken.CameraName = cam.Name;
                camToken.CreatedBy = milestoneUsername;

                database.InsertCreatedTokens(DateTime.Now, milestoneUsername, camToken.Token, request.CameraId, CameraToken.TokenVideoType.Live.ToString(), request.ExpireAfter, null, null, null);

                response = new TokenResponse(camToken.Token, request)
                {
                    CameraName = cam.Name,
                    ServerLink = Config.Instance.Server
                };
            }
            else if(request.ReqType == TokenRequest.RequestType.Recorded && (request.ExportTo - request.ExportFrom).TotalMinutes > 10)
            {
                response = new TokenResponse() { Error = "Exported duration is too long, max is 10 minutes." };
            }
            else if (request.ReqType == TokenRequest.RequestType.Recorded)
            {
                var exportJob = videoExporter.GetExportJob(
                    cam,
                    request.ExportFrom,
                    request.ExportTo);
                exportJob.OnLog += ExportJob_OnLog;

                EventHandler<ExportProgress> reportHandler =  (s, e) =>
                {
                    string progressPayload = null;
                    if (e.Error != null)
                    {
                        progressPayload = Utils.Packer.Serialize(new TokenProgress(e.Error, request));
                    }
                    else
                    {
                        progressPayload = Utils.Packer.Serialize(new TokenProgress(e.Progress, e.Message, request));
                    }

                    msgCom.TransmitMessage(new Message(Shared.Constants.Messaging.PrivateProgressMessageId, progressPayload), message.ExternalMessageSourceEndPoint, null, null);
                };

                exportJob.OnReportProgress += reportHandler;
                var exportedFile = exportJob.ExportAndConvert();
                exportJob.OnReportProgress -= reportHandler;
                exportJob.OnLog -= ExportJob_OnLog;

                if (exportedFile == null)
                {
                    response = new TokenResponse() { Error = "There was an error exporting the video file" };
                }
                else
                {
                    camToken = tokenManager.CreateNewRecordedToken(exportedFile, request.ExpireAfter, request.Password, request.Comment, request.RequireBankID, request.AccessGroups);
                    camToken.CameraName = cam.Name;
                    camToken.CreatedBy = milestoneUsername;

                    database.InsertCreatedTokens(DateTime.Now, milestoneUsername, camToken.Token, request.CameraId, CameraToken.TokenVideoType.Recorded.ToString(), request.ExpireAfter, null, request.ExportFrom, request.ExportTo);

                    camToken.TokenExpired += ExportCamToken_TokenExpired;
                    response = new TokenResponse(camToken.Token, request)
                    {
                        CameraName = cam.Name,
                        ServerLink = Config.Instance.Server
                    };
                }
            }

            response.Request = request;
            payload = Shared.Utils.Packer.Serialize(response);

            msgCom.TransmitMessage(new Message(Shared.Constants.Messaging.PrivateMessageId, payload), message.ExternalMessageSourceEndPoint, null, null);
        }

        private void ExportJob_OnLog(object sender, string e)
        {
            var ej = sender as ExportJob;
            Log("Export job (" + ej.Camera.Name + ", " + ej.From + " to " + ej.To +"): " + e);
        }

        private void ExportCamToken_TokenExpired(object sender, EventArgs e)
        {
            var camToken = sender as CameraToken;
            try
            {
                File.Delete(camToken.VideoFile);
            }
            catch
            {
                Log("Error deleting " + camToken.VideoFile + ", token: " + camToken.Token);
            }
        }

        private object GlobalFeatureRequestMessageCommunication(Message message, FQID dest, FQID source)
        {
            if(message.Data is string)
            {
                var payload = (string)message.Data;

                var featureRequest = Utils.Packer.Deserialize<FeatureRequest>(payload);

                if(featureRequest != null)
                {
                    BroadcastFeatures(message.ExternalMessageSourceEndPoint);
                }
            }

            return null;
        }

        private void BroadcastFeatures(FQID dst)
        {
            var featureRequest = new FeatureRequest()
            {
                CanSendSMS = LicenseSystem.LastCheck.license?.SMS == 1,
                CanShareLive = licenseIsValid,
                CanShareRecorded = licenseIsValid,
                LicenseIsValid = licenseIsValid,
                DefaultValidMinutes = 5,
                MaxValidMinutes = 60,
                CanUseBankID = LicenseSystem.LastCheck.license?.BankID == 1
            };

            msgCom.TransmitMessage(
                new Message(Constants.Messaging.PrivateFeatureRequestMessageId, Utils.Packer.Serialize(featureRequest)),
                dst, null, null);
        }

        private object GlobalAccessGroupsRequestMessageCommunication(Message message, FQID dest, FQID source)
        {
            BroadcastAccessGroups(message.ExternalMessageSourceEndPoint);

            return null;
        }

        private void BroadcastAccessGroups(FQID dst)
        {
            var groups = new List<AccessGroup>(database.AccessDatabaseManagement.AccessGroups);
            var sharedGroups = new List<SharedAccessGroup>();

            groups.ForEach(grp => sharedGroups.Add(new SharedAccessGroup(grp.Id, grp.Name)));

            msgCom.TransmitMessage(
                new Message(Constants.Messaging.GlobalAccessGroupsResponseMessageId, Utils.Packer.Serialize(sharedGroups.ToArray())),
                dst, null, null);
        }

        private void AccessDatabaseManagement_AccessGroupsUpdated(object sender, AccessGroupsUpdatedEventArgs e)
        {
            if(e.ShouldUpdateClients)
                BroadcastAccessGroups(dst: null);
        }


        private void Log(string msg)
        {
            OnLog?.Invoke(this, msg);
        }

        public void Close()
        {
            foreach(var imageProvider in imageProviders)
            {
                imageProvider.Value.Close();
            }

            clientManager.Close();

            foreach(var obj in messageObjects)
            {
                msgCom.UnRegisterCommunicationFilter(obj);
            }

            msgCom.Dispose();
            MessageCommunicationManager.Stop(EnvironmentManager.Instance.MasterSite.ServerId);
            VideoOS.Platform.SDK.Environment.RemoveAllServers();
        }
    }

}
