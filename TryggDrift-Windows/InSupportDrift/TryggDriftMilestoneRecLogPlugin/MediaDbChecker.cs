using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InSupport.Drift.Plugins.MilestoneRec
{
    /// <summary>
    /// This baby had some potential but they ruined it. Obsolete, do not use.
    /// </summary>
    [Obsolete]
    public class MediaDbChecker
    {
        const string MediaDbPathsSettingsKey = "MilestoneRecMediaDbs";
        const string RecorderConfigPathSettingsKey = "MilestoneRecConfigPath";

        private string recorderConfigPath = @"C:\ProgramData\Milestone\XProtect Recording Server\RecorderConfig.xml";

        private string[] mediaDbDirectories;

        public MediaDbChecker(Dictionary<string, string> settings)
        {
            if (settings.ContainsKey(MediaDbPathsSettingsKey))
            {
                mediaDbDirectories = JsonConvert.DeserializeObject<string[]>(settings[MediaDbPathsSettingsKey]);
            }

            if (settings.ContainsKey(RecorderConfigPathSettingsKey))
            {
                recorderConfigPath = settings[RecorderConfigPathSettingsKey];
            }
        }

        public RecServer GetRecServer()
        {
            if (!File.Exists(recorderConfigPath))
                throw new FileNotFoundException("Recorder config not found, specify with 'MilestoneRecConfigPath' setting.");

            var recorderConfig = Xml.RecorderConfig.Deserialize(recorderConfigPath);

            var recServer = new RecServer()
            {
                Name = recorderConfig.recorder.displayname,
                Storages = new List<Storage>()
            };

            foreach (var storagePath in mediaDbDirectories)
            {
                recServer.Storages.Add(CheckStorage(storagePath));
            }

            return recServer;
        }

        Storage CheckStorage(string storagePath)
        {
            var archiveConfigPath = Path.Combine(storagePath, "config.xml");
            var archiveCachePath = Path.Combine(storagePath, "archives_cache.xml");
            if (File.Exists(archiveConfigPath) && File.Exists(archiveCachePath))
            { // Valid storage

                var config = Xml.ArchiveConfig.Deserialize(archiveConfigPath);
                var archives = Xml.ArchivesCache.Deserialize(archiveCachePath);

                DateTime archiveCacheLastUpdate = new FileInfo(archiveCachePath).LastWriteTime;

                var storage = new Storage()
                {
                    Name = config.description,
                    MaxSpace = config.max_size_in_mb,
                    //UsedSpace = usedSpace,
                    //RetentionTime = retentionDateTime,
                    Devices = new List<Device>(),
                    Updated = archiveCacheLastUpdate
                };

                if (archives.tables != null) // no data
                {
                    var deviceGroups = archives.tables.GroupBy(t => t.archive_source).Select(grp => new { Device = grp.Key, Name = grp.First().description, TotalSize = grp.Sum(tbl => tbl.size) }).ToList();

                    //Console.WriteLine("Max size: {0}", config.max_size_in_mb);
                    //Console.WriteLine("Used space: {0}", usedSpace);
                    //ulong actualUsedSpace = 0;




                    foreach (var device in deviceGroups)
                    {
                        //actualUsedSpace += device.TotalSize;
                        //Console.WriteLine("{0}: {1}", device.Name, device.TotalSize);
                        var mb = Math.Round((device.TotalSize / 1024.0) / 1024.0, 1);
                        storage.Devices.Add(new Device()
                        {
                            Name = device.Name,
                            UsedMegaBytes = mb,
                            ActualMegaBytes = mb
                        });
                    }
                    //Console.WriteLine("Actual size used: {0}", (actualUsedSpace / 1024) / 1024);
                }
                //storages.Add(storage);

                if (config.archive != null && config.archive.link != null)
                {
                    storage.Retention = TimeSpan.FromMinutes(config.archive.skip_minutes);
                    storage.Link = CheckStorage(config.archive.link.destination_bank);

                    //storage.Devices = storage.Devices.Join(storage.Link.Devices, outSel => outSel.Name, inSel => inSel.Name, (first, second) => 
                    //new Device() 
                    //{ 
                    //    Name = first.Name, 
                    //    UsedMegaBytes = first.ActualMegaBytes - second.ActualMegaBytes, 
                    //    ActualMegaBytes = first.ActualMegaBytes 
                    //}).ToList();
                }
                else
                {
                    storage.Retention = TimeSpan.FromMinutes(config.max_minutes);
                }
                return storage;

            }
            else
            {
                throw new FileNotFoundException(storagePath + " is not a valid storage path.");
            }
        }
    }

    public class RecServer
    {
        public string Name { get; set; }
        public List<Storage> Storages { get; set; }
    }

    public class Storage
    {
        public DateTime Updated { get; set; }
        public string Name { get; set; }
        public List<Device> Devices { get; set; }

        public ulong MaxSpace { get; set; }

        public Storage Link { get; set; }
        public TimeSpan Retention { get; set; }
    }
    public class Device
    {
        public string Name { get; set; }
        public double UsedMegaBytes { get; set; }

        [JsonIgnore]
        public double ActualMegaBytes { get; set; }
    }

    public static class Extensions
    {
        public static ulong Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, ulong> summer)
        {
            ulong total = 0;

            foreach (var item in source)
                total += summer(item);

            return total;
        }
    }
}
