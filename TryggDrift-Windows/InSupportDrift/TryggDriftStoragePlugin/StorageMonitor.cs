using InSupport.Drift.Monitor;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TryggDriftStoragePlugin;

namespace InSupport.Drift.Plugins
{
    public class StorageMonitor : BaseMonitor
    {
        public override float MonitorVersion => 1.2f;
        public override string MonitorName => "Storage";

        string[] drives;

        public StorageMonitor() : base()
        {

        }

        public override void LoadSettings(Dictionary<string, string> settings)
        {
            if (settings.ContainsKey(StoragePluginConfigWpf._CFG_KEY))
            {
                drives = JsonConvert.DeserializeObject<StoragePluginCfg>(settings["Drives"]).Drives;
            }
            else
            {
                this.Enabled = false;
            }
        }

        public StorageInfo[] StorageInformation
        {
            get
            {
                List<StorageInfo> storageInfos = new List<StorageInfo>();

                foreach (DriveInfo drive in DriveInfo.GetDrives())
                {
                    if (!drive.IsReady)
                        continue;

                    if (drives.Any(d => d == drive.Name))
                    {
                        storageInfos.Add(new StorageInfo(drive));
                    }

                    //if (drives != null)
                    //{
                    //    bool found = false;
                    //    foreach (var d in drives)
                    //    {
                    //        found = drive.Name.StartsWith(d);
                    //        if (found)
                    //            break;
                    //    }
                    //    if (!found)
                    //        continue;
                    //    storageInfos.Add(new StorageInfo(drive));
                    //}

                }
                return storageInfos.ToArray();
            }
        }
    }
    public class StorageInfo
    {
        public string Label { get; set; }
        public string Name { get; set; }
        public double PercentageFree { get; set; }
        public long FreeSpace { get; set; }
        public long TotalSpace { get; set; }

        public StorageInfo(DriveInfo drive)
        {
            this.Label = drive.VolumeLabel;
            this.Name = drive.Name;
            this.FreeSpace = drive.TotalFreeSpace;
            this.TotalSpace = drive.TotalSize;
            this.PercentageFree = ToPercentage(drive.TotalFreeSpace, drive.TotalSize);
        }

        static double ToPercentage(long avalible, long total)
        {
            return ((double)avalible / (double)total * 100);
        }

        //public static StorageInfo[] StorageInformation
        //{
        //    get
        //    {
        //        List<StorageInfo> storageInfos = new List<StorageInfo>();

        //        foreach (var drive in DriveInfo.GetDrives().Where(d => d.IsReady && d.DriveType != DriveType.CDRom))
        //        {
        //            if (!drive.IsReady)
        //                continue;

        //            storageInfos.Add(new StorageInfo(drive));
        //        }
        //        return storageInfos.ToArray();
        //    }
        //}
    }
}
