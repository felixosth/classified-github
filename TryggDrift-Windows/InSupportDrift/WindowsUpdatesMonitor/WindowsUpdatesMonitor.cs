using InSupport.Drift.Monitor;
using System;
using System.Collections.Generic;
using System.Linq;
using WUApiLib;

namespace InSupport.Drift.Plugins
{
    public class WindowsUpdatesMonitor : BaseMonitor
    {
        public override float MonitorVersion => 1.0f;

        public WinUpdate[] Updates => WinUpdate.GetNewUpdates();

        public override string MonitorName => "Windows Updates";

        public WindowsUpdatesMonitor() : base()
        {

        }

        public override void LoadSettings(Dictionary<string, string> settings)
        {
        }
    }

    public class WinUpdate
    {
        public string URL => myUpdate.SupportUrl;

        public string Title => myUpdate.Title;
        public string Description => myUpdate.Description;
        public bool IsMandatory => myUpdate.IsMandatory;
        public string[] KBArticleIDs => myUpdate.KBArticleIDs.Cast<string>().ToArray();
        public string RelaseNotes => myUpdate.ReleaseNotes;

        public string[] InstallationBehaviour => GetInstallationBehvaiour(myUpdate.InstallationBehavior);

        private string[] GetInstallationBehvaiour(IInstallationBehavior behaviour)
        {
            return new string[]
            {
                nameof(behaviour.RebootBehavior) + ": " + behaviour.RebootBehavior.ToString(),
                nameof(behaviour.RequiresNetworkConnectivity) + ": " + behaviour.RequiresNetworkConnectivity.ToString(),
                nameof(behaviour.Impact) + ": " + behaviour.Impact.ToString(),
                nameof(behaviour.CanRequestUserInput) + ": " + behaviour.CanRequestUserInput.ToString()
            };
        }

        public string Type => myUpdate.Type.ToString();
        public bool IsDownloaded => myUpdate.IsDownloaded;
        public bool IsInstalled => myUpdate.IsInstalled;
        private IUpdate myUpdate { get; set; }

        public WinUpdate(IUpdate update)
        {
            //myUpdate.Identity.UpdateID
            myUpdate = update;
        }

        public static WinUpdate[] GetNewUpdates() => SearchFromServer(ServerSelection.ssDefault).ToArray();

        public static WinUpdate[] SearchFromServer(ServerSelection server)
        {
            UpdateSession uSession = new UpdateSession();
            IUpdateSearcher uSearcher = uSession.CreateUpdateSearcher();
            uSearcher.ServerSelection = server;
            uSearcher.IncludePotentiallySupersededUpdates = false;
            uSearcher.Online = false;
            WinUpdate[] updates = new WinUpdate[0];

            try
            {
                var result = uSearcher.Search("IsInstalled=0");
                Console.WriteLine("Availible updates: " + result.Updates.Count);
                updates = new WinUpdate[result.Updates.Count];

                for (int i = 0; i < result.Updates.Count; i++)
                {
                    updates[i] = new WinUpdate(result.Updates[i]);
                }
            }
            catch
            {
            }

            return updates;
        }
    }
}
