using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TryggSTORE.Http
{
    public class ConfigFile
    {
        // Static
        public string OccupancyCameraIP { get; set; }
        public string OccupancyCameraUsername { get; set; }
        public string OccupancyCameraPassword { get; set; }

        public int MaxOccupancyThreshold { get; set; }
        public int CloseOccupancyThreshold { get; set; }

        public int HttpPort { get; set; }

        public bool HideMaxOccText { get; set; }

        public bool DebugLog { get; set; }

        public string SiteName { get; set; }

        public NotifyProfile[] NotifyProfiles { get; set; }

        public int CurrentCountOffset { get; set; }

        // Dynamic globals
        private int _currentCount;
        public int CurrentCount 
        { 
            get => _currentCount + CurrentCountOffset; 
            set 
            {
                if(_currentCount != value)
                {
                    _currentCount = value;
                    CurrentCountChanged?.Invoke(this, _currentCount);
                }
            }
        }

        public int MaxCountChange { get; set; }

        public event EventHandler<int> CurrentCountChanged;
        public bool EmergencyStop { get; set; }

        private ConfigFile() // Default settings
        {
            OccupancyCameraIP = "192.168.0.90";
            OccupancyCameraUsername = "root";
            OccupancyCameraPassword = "pass";

            HttpPort = 1266;
            DebugLog = false;

            MaxOccupancyThreshold = 100;
            CloseOccupancyThreshold = 90;
            CurrentCount = 0;
            CurrentCountOffset = 0;
            EmergencyStop = false;

            MaxCountChange = 25;

            HideMaxOccText = false;

            NotifyProfiles = new NotifyProfile[]
            {
                new NotifyProfile()
                {
                    EmailAddresses = new string[]{"example@domain.com"},
                    NotifyAtCount = 100,
                    UseDefaultSubjectAndMessage = false,
                    EmailMessage = "This is the current count: %CURCOUNT%!",
                    EmailSubject = "This is the subject!",
                    Enabled = false
                }
            };
        }
        
        public void UpdateValues(ConfigFile newConfig)
        {
            if(newConfig != null)
            {
                if (newConfig.OccupancyCameraIP != null)
                    this.OccupancyCameraIP = newConfig.OccupancyCameraIP;

                if (newConfig.OccupancyCameraUsername != null)
                    this.OccupancyCameraUsername = newConfig.OccupancyCameraUsername;

                if (newConfig.OccupancyCameraPassword != null)
                    this.OccupancyCameraPassword = newConfig.OccupancyCameraPassword;

                if (newConfig.MaxOccupancyThreshold != default)
                    this.MaxOccupancyThreshold = newConfig.MaxOccupancyThreshold;

                if (newConfig.CloseOccupancyThreshold != default)
                    this.CloseOccupancyThreshold = newConfig.CloseOccupancyThreshold;

                if (newConfig.SiteName != default)
                    this.SiteName = newConfig.SiteName;

            }
        }

        private static ConfigFile _config;
        public static ConfigFile Instance
        {
            get 
            {
                if(_config == null)
                {
                    if (File.Exists(GetPath()))
                    {
                        using (var fs = new FileStream(GetPath(), FileMode.Open, FileAccess.Read))
                        using(var sr = new StreamReader(fs))
                        {
                            var fileText = sr.ReadToEnd();
                            _config = JsonConvert.DeserializeObject<ConfigFile>(fileText);
                        }

                         _config.CurrentCount = 0;
                    }
                    else
                    {
                        _config = new ConfigFile();
                        _config.SaveConfig();
                    }
                }

                return _config;
            }
        }

        private static string GetPath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.cfg");
        }

        public void SaveConfig()
        {
            var fileData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this, Formatting.Indented));
            using(var fs = new FileStream(GetPath(), FileMode.Create, FileAccess.Write))
            {
                fs.Write(fileData, 0, fileData.Length);
            }
        }
    }

    public class NotifyProfile
    {
        public string[] EmailAddresses { get; set; }
        public int NotifyAtCount { get; set; }
        public string EmailMessage { get; set; }
        public string EmailSubject { get; set; }
        public bool Enabled { get; set; } = true;

        public bool UseDefaultSubjectAndMessage { get; set; }
    }
}
