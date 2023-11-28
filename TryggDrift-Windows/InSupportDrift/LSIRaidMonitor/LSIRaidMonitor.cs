using InSupport.Drift.Monitor;
using InSupport.Drift.Plugins.RaidMonitor;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace InSupport.Drift.Plugins
{
    public class LSIRaidMonitor : BaseMonitor
    {
        public override float MonitorVersion => 1.21f;
        string path { get; set; } = @"C:\Program Files (x86)\MegaRAID Storage Manager\StorCLI64.exe";

        public LSIRaidMonitor() : base()
        {

        }

        public override void LoadSettings(Dictionary<string, string> settings)
        {
            if (settings.ContainsKey("StorCLIPath"))
            {
                path = settings["StorCLIPath"];
            }
            //else if(!File.Exists(path))
            //    this.Enabled = false;
        }

        public VirtualDiskInfo VirtualDriveInfo
        {
            get
            {
                if (path != null)
                {
                    return JsonConvert.DeserializeObject<VirtualDiskInfo>(CmdCommand(path, "/call /vall show J"));
                }
                else
                    throw new FileNotFoundException(path);
            }
        }

        public DisksInfo DisksInfo
        {
            get
            {
                if (path != null)
                {
                    return JsonConvert.DeserializeObject<DisksInfo>(CmdCommand(path, "/call /eall /sall show J"));
                }
                else
                    throw new FileNotFoundException(path);
            }
        }

        private string CmdCommand(string command, string args)
        {
            Process p = new Process();

            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = command;
            p.StartInfo.Arguments = args;
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            return output;
        }

        public override string MonitorName => "RaidMonitor";
    }

}
