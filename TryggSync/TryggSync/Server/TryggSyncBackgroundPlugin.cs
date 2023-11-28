using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Xml;
using TryggSync.Client;
using VideoOS.Platform;
using VideoOS.Platform.Background;
using VideoOS.Platform.Client;
using VideoOS.Platform.Login;
using VideoOS.Platform.Messaging;

namespace TryggSync.Server
{
    /// <summary>
    /// A background plugin will be started during application start and be running until the user logs off or application terminates.<br/>
    /// The Environment will call the methods Init() and Close() when the user login and logout, 
    /// so the background task can flush any cached information.<br/>
    /// The base class implementation of the LoadProperties can get a set of configuration, 
    /// e.g. the configuration saved by the Options Dialog in the Smart Client or a configuration set saved in one of the administrators.  
    /// Identification of which configuration to get is done via the GUID.<br/>
    /// The SaveProperties method can be used if updating of configuration is relevant.
    /// <br/>
    /// The configuration is stored on the server the application is logged into, and should be refreshed when the ApplicationLoggedOn method is called.
    /// Configuration can be user private or shared with all users.<br/>
    /// <br/>
    /// This plugin could be listening to the Message with MessageId == Server.ConfigurationChangedIndication to when when to reload its configuration.  
    /// This event is send by the environment within 60 second after the administrator has changed the configuration.
    /// </summary>
    public class TryggSyncBackgroundPlugin : BackgroundPlugin
    {
        OperatorUser currentOwner = null;
        FQID ownerEndPoint;

        //FQID currentCamera = null;

        List<OperatorUser> viewers = new List<OperatorUser>();

        MessageCommunication messageCommunication;
        List<object> msgComObjs;

        public override void Init()
        {
            MessageCommunicationManager.Start(EnvironmentManager.Instance.MasterSite.ServerId);
            messageCommunication = MessageCommunicationManager.Get(EnvironmentManager.Instance.MasterSite.ServerId);


            msgComObjs = new List<object>()
            {
                messageCommunication.RegisterCommunicationFilter(HandleTryggSyncMsgCommandRequest, new CommunicationIdFilter(TryggSyncDefinition.TryggSyncMsgCommandRequest))
            };

            Log("Backend initialized");
        }

        void Log(string msg)
        {
            EnvironmentManager.Instance.Log(false, "TryggSync Backend", msg);
        }

        private object HandleTryggSyncMsgCommandRequest(VideoOS.Platform.Messaging.Message message, FQID dest, FQID source)
        {
            var command = Packer.DeserializeObject(message.Data as string) as TryggSyncMsgCommandRequest;

            OperatorUser op = null;
            //Log("Request recieved: " + command.MsgCommandType.ToString());

            switch(command.MsgCommandType)
            {
                case MsgCommandTypes.OwnershipStatus:

                    BroadcastOwnershipStatus(message.ExternalMessageSourceEndPoint);
                    break;

                case MsgCommandTypes.TakeOwnership:

                    if(currentOwner == null)
                    {
                        //currentOwner = Packer.DeserializeObject(command.RequestParameters[0] as string) as OperatorUser;
                        currentOwner = command.RequestParameters[0] as OperatorUser;
                        ownerEndPoint = message.ExternalMessageSourceEndPoint;
                        BroadcastOwnershipStatus(null);
                    }

                    break;

                case MsgCommandTypes.LeaveOwnership:

                    //op = Packer.DeserializeObject(command.RequestParameters[0] as string) as OperatorUser;
                    op = command.RequestParameters[0] as OperatorUser;
                    if (op.Equals(currentOwner))
                    {
                        currentOwner = null;
                        BroadcastOwnershipStatus(null);
                    }

                    break;
                case MsgCommandTypes.UpdateViewList:
                    op = command.RequestParameters[0] as OperatorUser;
                    //op = Packer.DeserializeObject(command.RequestParameters[0] as string) as OperatorUser;
                    bool isAdding = (bool)command.RequestParameters[1];
                    if (isAdding)
                        viewers.Add(command.RequestParameters[0] as OperatorUser);
                    else
                    {
                        foreach(var existingOp in viewers)
                        {
                            if (existingOp.Equals(op))
                            {
                                if(currentOwner != null)
                                {
                                    if (currentOwner.Equals(op))
                                    {
                                        currentOwner = null;
                                        BroadcastOwnershipStatus(null);
                                    }
                                }

                                viewers.Remove(existingOp);
                                break;
                            }
                        }
                        //viewers.Remove(command.RequestParameters[0] as OperatorUser);

                    }

                    new TryggSyncMsgCommandResponse(MsgCommandTypes.UpdateViewList, viewers).Respond(null, messageCommunication);

                    break;

                case MsgCommandTypes.CameraPlayback:

                    op = command.RequestParameters[0] as OperatorUser;

                    if(op.Equals(currentOwner))
                    {
                        //var camInfo = command.RequestParameters[1] as CameraPlaybackInfo;
                        //currentCamera = Item.Deserialize(command.RequestParameters[1] as string).FQID;
                        new TryggSyncMsgCommandResponse(MsgCommandTypes.CameraPlayback, command.RequestParameters[1]).Respond(null, messageCommunication);
                    }
                    break;

                case MsgCommandTypes.PainStroke:

                    op = command.RequestParameters[0] as OperatorUser;

                    if (op.Equals(currentOwner))
                    {
                        //var camInfo = command.RequestParameters[1] as CameraPlaybackInfo;
                        //currentCamera = Item.Deserialize(command.RequestParameters[1] as string).FQID;
                        new TryggSyncMsgCommandResponse(MsgCommandTypes.PainStroke, command.RequestParameters[1]).Respond(null, messageCommunication);
                    }

                    break;

                case MsgCommandTypes.JustConnectedInfo:

                    var fakeItem = new Item();
                    fakeItem.FQID = message.ExternalMessageSourceEndPoint;
                    new TryggSyncMsgCommandResponse(MsgCommandTypes.JustConnectedInfo, fakeItem.Serialize()).Respond(ownerEndPoint, messageCommunication);

                    break;
            }

            return null;
        }

        private void BroadcastOwnershipStatus(FQID endpoint)
        {
            new TryggSyncMsgCommandResponse(MsgCommandTypes.OwnershipStatus, currentOwner == null ? null : currentOwner).Respond(endpoint, messageCommunication);
        }


        public override void Close()
        {
            foreach (var obj in msgComObjs)
            {
                messageCommunication.UnRegisterCommunicationFilter(obj);
            }
        }

        public override List<EnvironmentType> TargetEnvironments
        {
            get { return new List<EnvironmentType>() { EnvironmentType.Service }; } // Default will run in the Event Server
        }

        public override String Name
        {
            get { return "TryggSync BackgroundPlugin"; }
        }

        public override Guid Id
        {
            get { return TryggSyncDefinition.TryggSyncBackgroundPlugin; }
        }
    }

    [Serializable]
    public class OperatorUser
    {
        public string DisplayName { get; set; }
        public Guid ID { get; set; }


        public OperatorUser(string name, Guid id)
        {
            this.DisplayName = name;
            this.ID = id;
        }


        public override string ToString() => DisplayName;

        public bool Equals(OperatorUser other)
        {
            return ID == other.ID && DisplayName == other.DisplayName;
        }
    }
}
