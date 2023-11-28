using InSupport.Drift.Monitor;
using InSupportDriftMilestonePlugin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using VideoOS.Platform;
using VideoOS.Platform.ConfigurationItems;
using VideoOS.Platform.Data;
using VideoOS.Platform.License;
using VideoOS.Platform.Messaging;

namespace InSupport.Drift.Plugins
{
    public class MilestoneMonitor : BaseMonitor
    {
        public override float MonitorVersion => 1.5f;

        public AcapStatusMonitor AcapStatusMonitor { get; set; } = new AcapStatusMonitor();

        private Dictionary<string, CameraEventTimestamps> cameraEventTimestamps;
        public Dictionary<string, CameraEventTimestamps> InactiveCameras
        {
            get
            {
                if (cameraEventTimestamps == null)
                    return null;
                var inactiveCameras = new Dictionary<string, CameraEventTimestamps>();
                foreach (var cam in cameraEventTimestamps)
                {
                    var inactiveEvents = new Dictionary<string, EventTimestamp>();
                    foreach (var camEvent in cam.Value.Events)
                    {
                        if (camEvent.Value.Alarm)
                            inactiveEvents[camEvent.Key] = camEvent.Value;
                    }
                    if (inactiveEvents.Any())
                        inactiveCameras[cam.Key] = new CameraEventTimestamps(cam.Value.CameraName, inactiveEvents);
                }
                return inactiveCameras.Any() ? inactiveCameras : null;
            }
        }

        private MessageCommunication MsgCom { get; set; }
        private object stateComObj, alarmComObj, eventComObj;
        string apiUri;

        private int NotRespondingThreshold { get; set; } = 3;

        private List<MilestoneCamera> CamerasDownLastCheck { get; set; }

        public string SLC { get; set; }
        public int ProductCode { get; set; }
        public string ProductVersion { get; set; }

        private bool IncludeRecordedSequences { get; set; }

        public MilestoneMonitor() : base()
        {
        }

        public override void LoadSettings(Dictionary<string, string> settings)
        {
            if (settings.ContainsKey("MilestoneServer"))
            {
                CamerasDownLastCheck = new List<MilestoneCamera>();

                if (settings.ContainsKey("MilestoneNotResThreshold") && int.TryParse(settings["MilestoneNotResThreshold"], out int tmpNRT))
                {
                    NotRespondingThreshold = tmpNRT;
                }

                VideoOS.Platform.SDK.Environment.Initialize();

                var server = new Uri(settings["MilestoneServer"]);

                if (settings.ContainsKey("MilestoneUseCurrent"))
                {
                    NetworkCredential nc = CredentialCache.DefaultNetworkCredentials;
                    VideoOS.Platform.SDK.Environment.AddServer(secureOnly: false, server, nc);
                }
                else if (settings.ContainsKey("MilestoneUsername") && settings.ContainsKey("MilestonePassword"))
                {
                    CredentialCache cc = VideoOS.Platform.Login.Util.BuildCredentialCache(
                        server,
                        StringCipher.Decrypt(settings["MilestoneUsername"], MilestoneConfigUserControlWpf.EncryptKey),
                        StringCipher.Decrypt(settings["MilestonePassword"], MilestoneConfigUserControlWpf.EncryptKey),
                        "Basic");

                    VideoOS.Platform.SDK.Environment.AddServer(secureOnly: false, server, cc);
                }

                if (!settings.ContainsKey("MilestoneExcludeSequences") || bool.Parse(settings["MilestoneExcludeSequences"]) == false)
                {
                    VideoOS.Platform.SDK.Export.Environment.Initialize();
                    IncludeRecordedSequences = true;
                }

                VideoOS.Platform.SDK.Environment.Login(server);

                if (VideoOS.Platform.SDK.Environment.IsLoggedIn(server))
                {
                    var licMgr = new LicenseManager(EnvironmentManager.Instance.MasterSite.ServerId.UserContext);
                    SLC = licMgr.SLC;
                    ProductCode = licMgr.ProductCode;

                    try
                    {
                        // Get the version of Milestone (I.E. 21.1b)
                        List<Item> lServ = Configuration.Instance.GetItemsByKind(Kind.Server);
                        Item serverItem = lServ[0];
                        ProductVersion = serverItem.Properties["ProductVersion"];
                    }
                    catch
                    {
                        ProductVersion = "N/A";
                    }

                    MessageCommunicationManager.Start(EnvironmentManager.Instance.MasterSite.ServerId);
                    MsgCom = MessageCommunicationManager.Get(EnvironmentManager.Instance.MasterSite.ServerId);

                    stateComObj = MsgCom.RegisterCommunicationFilter(ProvideCurrentStateResponseHandler,
                        new VideoOS.Platform.Messaging.CommunicationIdFilter(MessageCommunication.ProvideCurrentStateResponse));

                    if (settings.ContainsKey("MilestoneAlarmList") && settings["MilestoneAlarmList"].ToLower() == "true")
                    {
                        apiUri = settings["DriftUrl"] + "/backend/api.php";

                        alarmComObj = MsgCom.RegisterCommunicationFilter(NewAlarmIndicationHandler,
                            new VideoOS.Platform.Messaging.CommunicationIdFilter(VideoOS.Platform.Messaging.MessageId.Server.NewAlarmIndication));
                    }

                    if (settings.ContainsKey("MilestoneEventConfig"))
                    {
                        eventComObj = MsgCom.RegisterCommunicationFilter(NewEventHandler, new CommunicationIdFilter(MessageId.Server.NewEventIndication));
                        cameraEventTimestamps = CameraEventTimestamps.FromConfig(
                            JsonConvert.DeserializeObject<MilestoneEventConfig>(settings["MilestoneEventConfig"])
                            );
                    }

                    AcapStatusMonitor.Init();
                }
                else
                {
                    throw new Exception("Failed to login to Milestone");
                }
            }
        }

        private object NewEventHandler(Message message, FQID destination, FQID sender)
        {
            if (message.Data as BaseEvent is BaseEvent baseEventData)
                UpdateEventTimestamp(baseEventData);

            return null;
        }

        public override void Close()
        {
            try
            {
                VideoOS.Platform.SDK.Environment.Logout();
                if (MsgCom != null)
                {
                    if (stateComObj != null)
                        MsgCom.UnRegisterCommunicationFilter(stateComObj);

                    if (alarmComObj != null)
                        MsgCom.UnRegisterCommunicationFilter(alarmComObj);

                    if (eventComObj != null)
                        MsgCom.UnRegisterCommunicationFilter(eventComObj);
                }
                AcapStatusMonitor.Close();
                MsgCom?.Dispose();
            }
            catch
            {
            }
        }

        private List<MilestoneCamera> cameras;

        private object ProvideCurrentStateResponseHandler(VideoOS.Platform.Messaging.Message message, FQID dest, FQID source)
        {
            Configuration.Instance.RefreshConfiguration(Kind.Camera);
            Configuration.Instance.RefreshConfiguration(Kind.Speaker);

            var unfilteredItems = Configuration.Instance.GetItemsByKind(Kind.Camera, ItemHierarchy.SystemDefined);
            unfilteredItems.AddRange(Configuration.Instance.GetItemsByKind(Kind.Speaker, ItemHierarchy.SystemDefined));

            var camerasAndSpeakers = GetAllItems(unfilteredItems);
            cameras = new List<MilestoneCamera>();

            if (message.Data as Collection<ItemState> is Collection<ItemState> result)
            {
                foreach (var item in camerasAndSpeakers)
                {
                    var itemState = result.FirstOrDefault(r => r.FQID.ObjectId == item.FQID.ObjectId);

                    if (itemState != null)
                    {
                        var msCam = new MilestoneCamera(item, "Responding");

                        // Find serialnumber
                        // Only works in C-code Milestone
                        try
                        {
                            Camera camera = new Camera(item.FQID);
                            Hardware hardware = new Hardware(EnvironmentManager.Instance.CurrentSite.ServerId, camera.ParentItemPath);

                            var hardwareProperties = hardware.HardwareDriverSettingsFolder.HardwareDriverSettings.FirstOrDefault().HardwareDriverSettingsChildItems.FirstOrDefault().Properties;

                            msCam.Model = hardware.Model;
                            msCam.SerialNumber = hardwareProperties.GetValue("SerialNumber");
                            msCam.FirmwareVersion = hardwareProperties.GetValue("FirmwareVersion");
                            msCam.Address = hardware.Address;
                        }
                        catch { }


                        if (itemState.State == "Not Responding")
                        {
                            if (CamerasDownLastCheck.Count(c => c.ID == msCam.ID) >= NotRespondingThreshold)
                            {
                                msCam.Status = "Not Responding";
                            }
                            else
                            {
                                CamerasDownLastCheck.Add(msCam);
                            }
                        }
                        else if (CamerasDownLastCheck.Count > 0)
                        {
                            CamerasDownLastCheck.RemoveAll(c => c.ID == msCam.ID);
                        }

                        if (IncludeRecordedSequences)
                        {
                            try
                            {
                                msCam.LastDaySequences = GetSequencesPercentage(item);
                            }
                            catch
                            {
                                msCam.LastDaySequences = -1;
                            }
                        }

                        cameras.Add(msCam);
                    }
                }

                working = false;
            }

            return null;
        }


        SequenceDataSource sequenceDataSource;
        private int GetSequencesPercentage(Item cam)
        {
            var from = DateTime.Now.AddDays(-1);
            var toTimeSpan = TimeSpan.FromDays(1);
            sequenceDataSource = new SequenceDataSource(cam);
            var sequenceDataList = sequenceDataSource.GetData(from, TimeSpan.Zero, 0, toTimeSpan, int.MaxValue, DataType.SequenceTypeGuids.RecordingSequence).Cast<SequenceData>().Where(sd => sd.EventSequence.EndDateTime >= from);

            var type = sequenceDataList.GetType();
            TimeSpan interval = TimeSpan.Zero;

            foreach (SequenceData sequenceData in sequenceDataList)
            {
                interval += sequenceData.EventSequence.EndDateTime - (sequenceData.EventSequence.StartDateTime < from ? from : sequenceData.EventSequence.StartDateTime);
            }
            sequenceDataSource.Close();
            return (int)((interval.TotalSeconds / toTimeSpan.TotalSeconds) * 100);
        }

        private object NewAlarmIndicationHandler(Message message, FQID dest, FQID source)
        {
            if (message.Data is Alarm alarm)
            {
                SendData(apiUri, new { mguid = HostMonitor.GetMGUID(), action = "addToAlarmList", alarm = new { name = alarm.EventHeader.Name, type = "Milestone" } });
            }

            return null;
        }

        private string SendData(string uri, object data)
        {
            using (var client = new HttpClient())
            {
                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                var payload = JsonConvert.SerializeObject(data);
                var content = new StringContent(payload, Encoding.UTF8, "application/json");
                var result = client.PostAsync(uri, content).Result.Content.ReadAsStringAsync().Result;
                return result;
            }
        }

        static List<Item> GetAllItems(List<Item> items)
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

        public override string MonitorName => "Milestone";

        bool working = true;

        public MilestoneCamera[] Cameras
        {
            get
            {
                try
                {
                    working = true;
                    MsgCom.TransmitMessage(new Message(MessageCommunication.ProvideCurrentStateRequest), null, null, null);

                    // Wait for ProvideCurrentStateRequest to complete
                    while (working)
                        Thread.Sleep(25);

                    if (cameras == null || cameras.Count == 0)
                        return new MilestoneCamera[] { new MilestoneCamera("No cameras found") };
                    else
                        return cameras.ToArray();
                }
                catch (Exception ex)
                {
                    return new MilestoneCamera[] { new MilestoneCamera(ex.InnerException?.Message ?? ex.Message) };
                }
            }
        }

        private void UpdateEventTimestamp(BaseEvent baseEvent)
        {
            string eventId = baseEvent.EventHeader.Name;
            string sourceId = baseEvent.EventHeader.Source.FQID.ObjectId.ToString();
            if (cameraEventTimestamps != null && cameraEventTimestamps.ContainsKey(sourceId) && cameraEventTimestamps[sourceId].Events.ContainsKey(eventId))
                cameraEventTimestamps[sourceId].UpdateTimestamp(eventId);
        }
    }

    public class MilestoneCamera
    {
        public string Name { get; set; }
        public string Status { get; set; }

        public string ID { get; set; }

        public int? LastDaySequences { get; set; } = null;

        public string SerialNumber { get; set; }

        public string Model { get; set; }
        public string FirmwareVersion { get; set; }
        public string Address { get; set; }

        public MilestoneCamera(Item item, string state)
        {
            Name = item.Name;
            ID = item.FQID.ObjectId.ToString();
            Status = state;
        }

        public MilestoneCamera(string error)
        {
            Name = "Error";
            Status = error;
        }

    }
}
