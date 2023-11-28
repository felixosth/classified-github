using InSupport.Drift.Monitor;
using System;
using System.Collections.Generic;
using System.IO;

namespace LastModifiedFolderMonitor
{
    public class FolderMonitor : BaseMonitor
    {
        string pathToFolder { get; set; }
        public FolderMonitor() : base()
        {

        }

        public override void LoadSettings(Dictionary<string, string> settings)
        {
            if (settings.ContainsKey("MonitorLastModifiedFolder"))
            {
                pathToFolder = settings["MonitorLastModifiedFolder"];
            }
        }

        public string FolderPath => pathToFolder;

        public DateTime LastModified
        {
            get
            {
                try
                {
                    var di = new DirectoryInfo(pathToFolder);
                    return di.LastWriteTime;
                }
                catch
                {
                    return DateTime.MinValue;
                }
            }
        }

        public override float MonitorVersion => 1.0f;
        public override string MonitorName => "FolderMonitor";
    }
}
