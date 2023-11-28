using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using VideoOS.Platform;
using VideoOS.Platform.Messaging;
using XProtectWebStream.Shared;
using XProtectWebStreamPlugin.UI;

namespace XProtectWebStreamPlugin
{
    public class ClientCommunication
    {
        MessageCommunication msgCom;

        private List<object> msgComObjects = new List<object>();
        private List<object> envComObjects = new List<object>();

        Dictionary<Guid, TokenResponse> responseButtons = new Dictionary<Guid, TokenResponse>();
        Dictionary<string, Guid> progressMessages = new Dictionary<string, Guid>();

        internal FeatureRequest LastFeatureResponse { get; private set; }

        internal ObservableCollection<SharedAccessGroup> AccessGroups { get; private set; } = new ObservableCollection<SharedAccessGroup>();

        internal ObservableCollection<TokenResponse> ResponseHistory { get; private set; } = new ObservableCollection<TokenResponse>();

        internal ClientCommunication()
        {
            LastFeatureResponse = new FeatureRequest()
            {
                CanSendSMS = false,
                CanShareLive = false,
                CanShareRecorded = false,
                LicenseIsValid = false,
                CanUseBankID = false
            };

            MessageCommunicationManager.Start(EnvironmentManager.Instance.MasterSite.ServerId);
            msgCom = MessageCommunicationManager.Get(EnvironmentManager.Instance.MasterSite.ServerId);

            msgCom.ConnectionStateChangedEvent += MsgCom_ConnectionStateChangedEvent;

            RegisterMsgCom(PrivateMessageCommunication, Constants.Messaging.PrivateMessageId);
            RegisterMsgCom(PrivateProgressMessageCommunication, Constants.Messaging.PrivateProgressMessageId);

            RegisterMsgCom(FeatureRequestMessageCommunication, Constants.Messaging.PrivateFeatureRequestMessageId, waitForRegistration: true);
            RegisterMsgCom(AccessGroupsRequestMessageCommunication, Constants.Messaging.GlobalAccessGroupsResponseMessageId, waitForRegistration: true);

            RegisterEnvCom(MessageButtonClicked, MessageId.SmartClient.SmartClientMessageButtonClickedIndication);
            RegisterEnvCom(MessageRemoved, MessageId.SmartClient.SmartClientMessageRemovedIndication);
        }

        private void RegisterMsgCom(MessageReceiver messageReceiver, string messageId, bool waitForRegistration = false)
        {
            var msgIdFilter = new CommunicationIdFilter(messageId);
            var obj = msgCom.RegisterCommunicationFilter(messageReceiver, msgIdFilter);
            if (waitForRegistration)
                msgCom.WaitForCommunicationFilterRegistration(msgIdFilter);
            msgComObjects.Add(obj);
        }

        private void RegisterEnvCom(MessageReceiver messageReceiver, string messageId)
        {
            var msgIdFilter = new MessageIdFilter(messageId);
            var obj = EnvironmentManager.Instance.RegisterReceiver(messageReceiver, msgIdFilter);
            envComObjects.Add(obj);
        }

        private void MsgCom_ConnectionStateChangedEvent(object sender, EventArgs e)
        {
            EnvironmentManager.Instance.Log(false, "TryggSHARE", "MsgCom.IsConnected = " + msgCom.IsConnected);
        }

        public void Close()
        {
            msgCom.ConnectionStateChangedEvent -= MsgCom_ConnectionStateChangedEvent;

            msgComObjects.ForEach(obj => msgCom.UnRegisterCommunicationFilter(obj));
            msgCom.Dispose();
            envComObjects.ForEach(obj => EnvironmentManager.Instance.UnRegisterReceiver(obj));
        }

        public void RequestLiveToken(string cameraId, TimeSpan expiration, string password, string comment, bool requireBankId, int[] accessGroups)
        {
            ThreadPool.QueueUserWorkItem((state) =>
            {
                var requestToken = Utils.GetTimeToken();

                var tokenRequest = new TokenRequest()
                {
                    CameraId = cameraId,
                    ExpireAfter = expiration,
                    ReqType = TokenRequest.RequestType.Live,
                    RequestTimestamp = DateTime.Now,
                    Password = password,
                    Comment = !string.IsNullOrWhiteSpace(comment) ? comment : null,
                    RequestToken = requestToken,
                    RequireBankID = requireBankId,
                    AccessGroups = accessGroups
                };

                var payload = Utils.Packer.Serialize(tokenRequest);

                msgCom.TransmitMessage(new Message(Constants.Messaging.GlobalMessageId, payload), null, null, null);
            });

        }

        public void RequestExportToken(string cameraId, TimeSpan expiration, DateTime from, DateTime to, string password, string comment, bool requireBankId, int[] accessGroups)
        {
            ThreadPool.QueueUserWorkItem((state) =>
            {
                var requestToken = Utils.GetTimeToken();

                var tokenRequest = new XProtectWebStream.Shared.TokenRequest()
                {
                    CameraId = cameraId,
                    ExpireAfter = expiration,
                    ReqType = TokenRequest.RequestType.Recorded,
                    ExportFrom = from,
                    ExportTo = to,
                    RequestTimestamp = DateTime.Now,
                    Password = password,
                    Comment = !string.IsNullOrWhiteSpace(comment) ? comment : null,
                    RequestToken = requestToken,
                    RequireBankID = requireBankId,
                    AccessGroups = accessGroups
                };

                var payload = XProtectWebStream.Shared.Utils.Packer.Serialize(tokenRequest);

                msgCom.TransmitMessage(new Message(Constants.Messaging.GlobalMessageId, payload), null, null, null);
            });
        }

        public void RevokeToken(string token)
        {
            ThreadPool.QueueUserWorkItem((state) =>
            {
                var tokenRequest = new TokenRequest()
                {
                    TokenToRevoke = token,
                    ReqType = TokenRequest.RequestType.Revoke
                };

                var payload = Utils.Packer.Serialize(tokenRequest);
                msgCom.TransmitMessage(new Message(Constants.Messaging.GlobalMessageId, payload), null, null, null);

            });
        }

        object PrivateProgressMessageCommunication(VideoOS.Platform.Messaging.Message _msg, FQID dest, FQID source)
        {
            try
            {
                var payload = (string)_msg.Data;

                var progress = Utils.Packer.Deserialize<TokenProgress>(payload);

                SmartClientMessageData messageData = new SmartClientMessageData();

                Guid id = default;

                if(progressMessages.ContainsKey(progress.Request.RequestToken))
                {
                    id = progressMessages[progress.Request.RequestToken];
                }
                else
                {
                    id = Guid.NewGuid();
                    progressMessages[progress.Request.RequestToken] = id;
                }
                messageData.MessageId = id;
                messageData.IsClosable = false;

                if (progress.Error != null)
                {
                    messageData.IsClosable = true;
                    messageData.Message = progress.Error;
                    messageData.MessageType = SmartClientMessageDataType.Task;
                    messageData.TaskState = SmartClientMessageDataTaskState.Failed;
                }
                else
                {
                    messageData.Message = "Processing request";
                    messageData.TaskProgress = progress.Progress >= 0 && progress.Progress <= 100 ? (progress.Progress/100.0) : 1.0;
                    messageData.TaskText = progress.Message;
                    messageData.MessageType = SmartClientMessageDataType.Task;
                    messageData.TaskState = SmartClientMessageDataTaskState.Running;
                }

                EnvironmentManager.Instance.SendMessage(new Message(MessageId.SmartClient.SmartClientMessageCommand, messageData));

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return null;
        }

        object MessageRemoved(Message msg, FQID dst, FQID src)
        {
            if (msg.Data is Guid && responseButtons.ContainsKey((Guid)msg.Data))
            {
                responseButtons.Remove((Guid)msg.Data);
            }

            return null;
        }

        object MessageButtonClicked(Message msg, FQID dst, FQID src)
        {
            if (msg.Data is Guid && responseButtons.ContainsKey((Guid)msg.Data))
            {
                var response = responseButtons[(Guid)msg.Data];

                var window = new Window();

                window.Owner = Application.Current.MainWindow;
                window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                window.ResizeMode = ResizeMode.NoResize;
                window.Title = "Webstream link";
                window.SizeToContent = SizeToContent.WidthAndHeight;
                window.Width = 700;
                window.Height = 100;

                window.Content = new ShowLinkDialog(this, response.CompleteLink, response.Token);

                window.ShowDialog();

                //EnvironmentManager.Instance.SendMessage(new Message(MessageId.SmartClient.SmartClientMessageCommand, new SmartClientMessageData()
                //{
                //    MessageId = msg.Data,
                //    Message = string.Empty,
                //}));

                //responseButtons.Remove((Guid)msg.Data);
            }
            return null;
        }

        object PrivateMessageCommunication(VideoOS.Platform.Messaging.Message message, FQID dest, FQID source)
        {
            var payload = (string)message.Data;

            var response = Utils.Packer.Deserialize<TokenResponse>(payload);

            if (response.Request.ReqType == TokenRequest.RequestType.Revoke)
            {
                var prevLink = ResponseHistory.FirstOrDefault(r => r.Token == response.Token);
                if(prevLink != null)
                {
                    if(response.IsRevoked == false) // It doesnt exist on the server
                    {
                        ClientControl.Instance.CallOnUiThread(() => ResponseHistory.Remove(prevLink));
                    }
                    else
                        prevLink.IsRevoked = true;
                }
            }
            else
            {
                SmartClientMessageData messageData = new SmartClientMessageData();
                Guid id = default;
                if (progressMessages.ContainsKey(response.Request.RequestToken))
                {
                    id = progressMessages[response.Request.RequestToken];
                }
                else
                {
                    id = Guid.NewGuid();
                    progressMessages[response.Request.RequestToken] = id;
                }

                responseButtons[id] = response;
                messageData.MessageId = id;

                messageData.IsClosable = true;
                messageData.MessageType = SmartClientMessageDataType.Task;
                messageData.TaskProgress = 1.0;

                if (response.Error != null)
                {
                    messageData.Message = response.Error;
                    messageData.ButtonText = "";
                    messageData.TaskState = SmartClientMessageDataTaskState.Failed;
                }
                else
                {
                    messageData.Message = "The requested link is ready";
                    messageData.ButtonText = "View link";
                    messageData.TaskState = SmartClientMessageDataTaskState.Complete;
                }


                EnvironmentManager.Instance.SendMessage(new Message(MessageId.SmartClient.SmartClientMessageCommand, messageData));

                ClientControl.Instance.CallOnUiThread(() => ResponseHistory.Add(response));
                
                progressMessages.Remove(response.Request.RequestToken);
            }

            return null;
        }

        internal void SendEmail(string recipient, string token, string fullLink)
        {
            msgCom.TransmitMessage(
                new Message(Constants.Messaging.GlobalLinkSenderMessageId,
                Utils.Packer.Serialize(new SendLinkRequest(recipient, token, fullLink, SendLinkRequest.SendLinkType.Email))), 
                null, null, null);
        }

        internal void SendSMS(string recipient, string token, string fullLink)
        {
            msgCom.TransmitMessage(
                new Message(Constants.Messaging.GlobalLinkSenderMessageId,
                Utils.Packer.Serialize(new SendLinkRequest(recipient, token, fullLink, SendLinkRequest.SendLinkType.SMS))),
                null, null, null);
        }


        internal void RequestFeaturesFromServer()
        {
            msgCom.TransmitMessage(
                new Message(Constants.Messaging.GlobalFeatureRequestMessageId, Utils.Packer.Serialize(new FeatureRequest())),
                null, null, null);
        }

        internal void RequestAccessGroups()
        {
            msgCom.TransmitMessage(
                new Message(Constants.Messaging.GlobalAccessGroupsRequestMessageId),
                null, null, null);
        }

        object FeatureRequestMessageCommunication(Message msg, FQID dst, FQID src)
        {
            if(msg.Data is string)
            {
                var payload = (string)msg.Data;

                var featureResponse = Utils.Packer.Deserialize<FeatureRequest>(payload);

                if(featureResponse != null && featureResponse.LicenseIsValid != null)
                    LastFeatureResponse = featureResponse;
            }

            return null;
        }

        object AccessGroupsRequestMessageCommunication(Message msg, FQID dst, FQID src)
        {
            if (msg.Data is string)
            {
                var payload = (string)msg.Data;

                var newAccessGroups = Utils.Packer.Deserialize<SharedAccessGroup[]>(payload);


                var groupsToAdd = new List<SharedAccessGroup>();
                var foundItems = new List<SharedAccessGroup>();

                foreach(var newAccessGroup in newAccessGroups)
                {
                    var existingGroup = AccessGroups.FirstOrDefault(grp => grp.Id == newAccessGroup.Id);
                    if (existingGroup == null)
                    {
                        groupsToAdd.Add(newAccessGroup);
                    }
                    else
                    {
                        foundItems.Add(existingGroup);
                        if (newAccessGroup.Name != existingGroup.Name)
                        {
                            newAccessGroup.Name = existingGroup.Name;
                        }
                    }
                }

                var groupsToRemove = AccessGroups.Except(foundItems).ToList();
                groupsToRemove.ForEach(grp => AccessGroups.Remove(grp));
                //AccessGroups.RemoveAll(gpr => foundItems.Contains(gpr) == false);
                groupsToAdd.ForEach(grp => AccessGroups.Add(grp));
            }

            return null;
        }



    }
}
