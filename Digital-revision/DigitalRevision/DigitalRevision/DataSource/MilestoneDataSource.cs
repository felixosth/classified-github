using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VideoOS.Platform;
using VideoOS.Platform.ConfigurationItems;
using VideoOS.Platform.Data;
using VideoOS.Platform.SDK.Platform;
using static DigitalRevision.Global.Helper;


namespace DigitalRevision.DataSource
{
    class MilestoneDataSource : DataSourceBase
    {
        public override string Name => "MilestoneData";

        public override double Version => 0.1;

        // Collecting firmware from cameras and speakers, camera snapshots from Milestone

        public override async Task CollectData(string folderDestination)
        {
            ProgressIsIndeterminate = true;
            var csvSep = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;
            Login();
            var testFilePath = Path.Combine(folderDestination, "DigitalRevisionReport.csv");
            var snapshotPath = Path.Combine(folderDestination, "Snapshots");
            Directory.CreateDirectory(snapshotPath);

            var unfilteredItems = Configuration.Instance.GetItemsByKind(Kind.Camera, ItemHierarchy.SystemDefined);
            unfilteredItems.AddRange(Configuration.Instance.GetItemsByKind(Kind.Speaker, ItemHierarchy.SystemDefined));
            var camerasAndSpeakers = GetAllItems(unfilteredItems);

            using (var fs = new FileStream(testFilePath, FileMode.OpenOrCreate, FileAccess.Write))
            using (var writer = new StreamWriter(fs, Encoding.UTF8))
            {
                await writer.WriteLineAsync($"cameraName{csvSep}model{csvSep}serialNumber{csvSep}firmwareVersion{csvSep}address{csvSep}macAddress");

                JPEGVideoSource videoSource = null;
                for (int i = 0; i < camerasAndSpeakers.Count; i++)
                {
                    IConfigurationItem configItem = null;
                    if (camerasAndSpeakers[i].FQID.Kind == Kind.Camera)
                    {
                        configItem = new Camera(camerasAndSpeakers[i].FQID);
                    }
                    else if (camerasAndSpeakers[i].FQID.Kind == Kind.Speaker)
                    {
                        configItem = new Speaker(camerasAndSpeakers[i].FQID);
                    }
                    Hardware hardware = new Hardware(EnvironmentManager.Instance.CurrentSite.ServerId, configItem.ParentItemPath);

                    var hardwareProperties = hardware.HardwareDriverSettingsFolder.HardwareDriverSettings.FirstOrDefault().HardwareDriverSettingsChildItems.FirstOrDefault().Properties;
                    var model = hardware.Model;
                    var serialNumber = hardwareProperties.GetValue("SerialNumber");
                    var firmwareVersion = hardwareProperties.GetValue("FirmwareVersion");
                    var address = hardware.Address;
                    var macAddress = hardwareProperties.GetValue("MacAddress");

                    await writer.WriteLineAsync($"{StringToCSVCell(camerasAndSpeakers[i].Name)}{csvSep}{StringToCSVCell(model)}{csvSep}{StringToCSVCell(serialNumber)}{csvSep}{StringToCSVCell(firmwareVersion)}{csvSep}{StringToCSVCell(address)}{csvSep}{StringToCSVCell(macAddress)}");

                    if (camerasAndSpeakers[i].FQID.Kind == Kind.Camera)
                    {
                        videoSource = new JPEGVideoSource(camerasAndSpeakers[i]);
                        videoSource.Init();
                        SaveImageToDisc(videoSource, camerasAndSpeakers[i], snapshotPath);
                        videoSource.Close();
                        videoSource = null;
                        ProgressPercentage = 100 / camerasAndSpeakers.Count * (i + 1);
                    }
                }
            }

            VideoOS.Platform.SDK.Environment.Logout();
            //VideoOS.Platform.SDK.Environment.UnInitialize();
            ProgressIsIndeterminate = false;
        }

        private void SaveImageToDisc(JPEGVideoSource videoSource, Item camera, string snapshotPath)
        {
            if (videoSource != null)
            {
                var cameraSnapshot = videoSource.GetNearest(DateTime.Now);
                var recentPicCast = cameraSnapshot as JPEGData;
                if (recentPicCast?.Bytes != null)
                {
                    var SaveAbleImage = recentPicCast.Bytes;
                    var snapshotDirectory = Path.Combine(snapshotPath, camera.Name + ".jpg");
                    File.WriteAllBytes(snapshotDirectory, SaveAbleImage);
                }
            }
        }

        private void Login()
        {
            VideoOS.Platform.SDK.Environment.Initialize();
            VideoOS.Platform.SDK.Export.Environment.Initialize();
            var serverAdress = new Uri("http://localhost");
            var credentials = new CredentialCache();
            credentials.Add(serverAdress, "Negotiate", CredentialCache.DefaultNetworkCredentials);
            VideoOS.Platform.SDK.Environment.AddServer(secureOnly: false, serverAdress, credentials);
            try
            {
                VideoOS.Platform.SDK.Environment.Login(serverAdress);
            }
            catch (ServerNotFoundMIPException)
            {
                ShowError("Milestone server not responding at " + serverAdress);
            }
            catch (InvalidCredentialsMIPException)
            {
                ShowError("Invalid Milestone server credentials");
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
    }
}
