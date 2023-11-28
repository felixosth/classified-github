using FlxHelperLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Xml;
using VideoOS.Platform;
using VideoOS.Platform.Background;
using VideoOS.Platform.Client;
using VideoOS.Platform.Data;
using VideoOS.Platform.Messaging;
using InSupport_LicenseSystem;

namespace OpenALPR_Milestone.Background
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
    public class OpenALPR_MilestoneBackgroundPlugin : BackgroundPlugin
    {

        private string ALPR_Company_ID { get; set; }
        private string ALPR_Site { get; set; }
        private DateTime ALPR_Last_Check { get; set; }
        private DateTime ALPR_Start_Date { get; set; }
        private List<Item> ALPR_Related_Cameras { get; set; }
        private Item ALPR_Event { get; set; }
        private float ALPR_Check_Interval { get; set; }
        private string ALPR_Notification_Method { get; set; }

        private bool Running { get; set; }

        LicenseWrapper licenseWrapper;
        

        private int LastReadPK { get; set; }

        CancellationTokenSource cts = new CancellationTokenSource();

        /// <summary>
        /// Gets the unique id identifying this plugin component
        /// </summary>
        public override Guid Id
        {
            get { return OpenALPR_MilestoneDefinition.OpenALPR_MilestoneBackgroundPlugin; }
        }

        /// <summary>
        /// The name of this background plugin
        /// </summary>
        public override String Name
        {
            get { return "OpenALPR_Milestone BackgroundPlugin"; }
        }

        /// <summary>
        /// Called by the Environment when the user has logged in.
        /// </summary>
        public override void Init()
        {
            ALPR_Check_Interval = 1;
            LastReadPK = -1;

            var myMguid = Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Cryptography", "MachineGuid", null) as string;
            licenseWrapper = new LicenseWrapper(myMguid);
            licenseWrapper.OnLicenseExpired += LicenseWrapper_OnLicenseExpired;
            licenseWrapper.OnLicenseCheck += LicenseWrapper_OnLicenseCheck;
            licenseWrapper.CheckLicense(false);

            if(licenseWrapper.LastLicenseCheck != LicenseCheckResult.Valid)
            {
                Logger.Log($"License status: {licenseWrapper.LastLicenseCheck.ToString()}. Closing...");
                return;
            }
            licenseWrapper.Init();

            var item = Configuration.Instance.GetItem(OpenALPR_MilestoneDefinition.OpenALPR_MilestoneConfigItemId, OpenALPR_MilestoneDefinition.OpenALPR_MilestoneKind);
            
            if (item != null)
            {
                if (item.Properties.ContainsKey(Admin.OpenALPR_MilestoneUserControl._company_id_key))
                    ALPR_Company_ID = item.Properties[Admin.OpenALPR_MilestoneUserControl._company_id_key];

                if (item.Properties.ContainsKey(Admin.OpenALPR_MilestoneUserControl._site_key))
                    ALPR_Site = item.Properties[Admin.OpenALPR_MilestoneUserControl._site_key];

                if (item.Properties.ContainsKey(Admin.OpenALPR_MilestoneUserControl._last_check_key))
                    ALPR_Last_Check = DateTime.Parse(item.Properties[Admin.OpenALPR_MilestoneUserControl._last_check_key]);

                if (item.Properties.ContainsKey(Admin.OpenALPR_MilestoneUserControl._method_key))
                    ALPR_Notification_Method = item.Properties[Admin.OpenALPR_MilestoneUserControl._method_key];

                if (item.Properties.ContainsKey(Admin.OpenALPR_MilestoneUserControl._event_key))
                    ALPR_Event = Configuration.Instance.GetItem(Guid.Parse(item.Properties[Admin.OpenALPR_MilestoneUserControl._event_key]), Kind.TriggerEvent);

                if (item.Properties.ContainsKey(Admin.OpenALPR_MilestoneUserControl._cameras_key))
                {
                    var camIds = item.Properties[Admin.OpenALPR_MilestoneUserControl._cameras_key].FlxHelper().FromBase64<List<Guid>>();

                    ALPR_Related_Cameras = new List<Item>();

                    foreach (var camId in camIds)
                    {
                        var camItem = Configuration.Instance.GetItem(camId, Kind.Camera);
                        ALPR_Related_Cameras.Add(camItem);
                    }

                    if (ALPR_Related_Cameras.Count == 0)
                        ALPR_Related_Cameras = null;
                }

                if (string.IsNullOrEmpty(ALPR_Company_ID))
                {
                    Logger.Log(string.Format("'{0}' is null or empty.", nameof(ALPR_Company_ID)), true);
                    return;
                }

                if (string.IsNullOrEmpty(ALPR_Notification_Method))
                {
                    Logger.Log(string.Format("'{0}' is null or empty, defaulting to Alarm", nameof(ALPR_Notification_Method)), true);
                    ALPR_Notification_Method = "alarm";
                }

                if (string.IsNullOrEmpty(ALPR_Site))
                {
                    Logger.Log(string.Format("'{0}' is null or empty.", nameof(ALPR_Site)), true);
                    return;
                }

                if (ALPR_Event == null)
                {
                    Logger.Log(string.Format("'{0}' is null, no event will be triggered.", nameof(ALPR_Event)), true);
                }

                if (ALPR_Related_Cameras == null)
                {
                    Logger.Log(string.Format("'{0}' is null.", nameof(ALPR_Related_Cameras)), true);
                    return;
                }

                if (ALPR_Last_Check == null)
                {
                    Logger.Log(string.Format("'{0}' is null.", nameof(ALPR_Last_Check)), true);
                    return;
                }

                ALPR_Start_Date = DateTime.UtcNow;
                //ALPR_Start_Date = Debugger.IsAttached ? DateTime.UtcNow.AddDays(-7) : DateTime.UtcNow;

                Logger.Log("Started.");

                new Thread(RunThread).Start();
            }
            else
            {
                Logger.Log("Configuration is null, background thread not started.", true);
            }
        }

        private void LicenseWrapper_OnLicenseCheck(object sender, LicenseCheckResult e)
        {
            if(e == LicenseCheckResult.Valid && !Running)
            {
                Logger.Log("License is now valid. Starting...");
                new Thread(RunThread).Start();
            }
        }

        private void LicenseWrapper_OnLicenseExpired(object sender, LicenseCheckResult e)
        {
            cts.Cancel();
            Logger.Log("Your license have expired, this plugin is shutting down. Contact InSupport for renewal.");
        }

        private void RunThread()
        {
            Running = true;
            while (!cts.Token.IsCancellationRequested)
            {
                try
                {
                    string url = string.Format("https://cloud.openalpr.com/api/search/plate?company_id={0}&start={2}&topn=25&order=desc", ALPR_Company_ID, ALPR_Site, ALPR_Start_Date.ToString("o"));
                    ALPRPlate[] searchPlateResult = new ALPRPlate[0];
                    bool gotError = false;

                    try
                    {
                        searchPlateResult = JsonConvert.DeserializeObject<ALPRPlate[]>(Get(url));
                    }
                    catch(WebException ex)
                    {
                        gotError = true;
                        Logger.Log("Error fetching LPR data: " + ex.Message);
                    }

                    if (searchPlateResult.Length > 0)
                    {

                        foreach (var plate in searchPlateResult)
                        {
                            if (plate.pk == LastReadPK)
                                break;

                            if (plate.fields.site.ToLower() != ALPR_Site.ToLower())
                                continue;

                            if (ALPR_Notification_Method == "alarm")
                                CreateAlarm(plate);
                            else
                                CreateEvent(plate);

                            Logger.Log("Found plate: " + plate.fields.best_plate);
                        }

                        LastReadPK = searchPlateResult[0].pk;
                    }


                    Thread.Sleep((int)(ALPR_Check_Interval * 1000) + (gotError ? 30 * 1000 : 0));
                }
                catch(Exception ex)
                {
                    Logger.Log(ex.ToString(), true);
                }
            }
            Running = false;
        }

        private string Get(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        private ReferenceList GetCameraReferenceList()
        {
            ReferenceList referenceList = new ReferenceList();

            foreach (var relatedCamera in ALPR_Related_Cameras)
            {
                referenceList.Add(new Reference()
                {
                    FQID = relatedCamera.FQID
                });
            }
            return referenceList;
        }

        private void CreateAlarm(ALPRPlate plate)
        {
            Alarm alarm = new Alarm()
            {
                EventHeader = new EventHeader()
                {
                    ID = Guid.NewGuid(),
                    Type = "PlateRecognized",
                    Timestamp = plate.fields.epoch_time_end.ToLocalTime(),
                    Message = plate.fields.best_plate,
                    CustomTag = "OpenALPR",
                    Source = new EventSource() { FQID = ALPR_Related_Cameras[0].FQID }
                },
                Description = "ALPR Alarm",
                ReferenceList = GetCameraReferenceList()
            };

            EnvironmentManager.Instance.SendMessage(new Message(MessageId.Server.NewAlarmCommand) { Data = alarm });
            TriggerEvent();
        }

        private void CreateEvent(ALPRPlate plate)
        {
            var eventheader = new EventHeader()
            {
                ID = Guid.NewGuid(),
                Type = "PlateRecognized",
                Timestamp = plate.fields.epoch_time_end.ToLocalTime(),
                Message = plate.fields.best_plate,
                CustomTag = "OpenALPR",
                Source = new EventSource() { FQID = ALPR_Related_Cameras[0].FQID }
            };

            AnalyticsEvent analyticsEvent = new AnalyticsEvent()
            {
                ReferenceList = GetCameraReferenceList(),
                EventHeader = eventheader,
                Location = "ALPR_Plugin",
                Description = "ALPR Plate recognition",
                Vendor = new Vendor() { Name = "OpenALPR" },
                ObjectList = new AnalyticsObjectList()
                {
                    new AnalyticsObject()
                    {
                        Confidence = double.Parse(plate.fields.best_confidence.Replace('.', ',')),
                        Value = plate.fields.best_plate,
                        AlarmTrigger = true,
                        Description = "OpenALPR recognition",
                        Name = "Car plate",
                        Type = "Plate"
                    }
                }
            };

            EnvironmentManager.Instance.SendMessage(new Message(MessageId.Server.NewEventCommand) { Data = analyticsEvent });
            TriggerEvent();
        }

        private void TriggerEvent()
        {
            if(ALPR_Event != null)
            {
                EnvironmentManager.Instance.SendMessage(new Message(MessageId.Control.TriggerCommand), ALPR_Event.FQID);
            }
        }

        /// <summary>
        /// Called by the Environment when the user log's out.
        /// You should close all remote sessions and flush cache information, as the
        /// user might logon to another server next time.
        /// </summary>
        public override void Close()
        {
            Logger.Log("Closing...");
            licenseWrapper.Close();
            cts.Cancel();
        }

        /// <summary>
        /// Define in what Environments the current background task should be started.
        /// </summary>
        public override List<EnvironmentType> TargetEnvironments
        {
            get { return new List<EnvironmentType>() { EnvironmentType.Service }; } // Default will run in the Event Server
        }

    }
}
