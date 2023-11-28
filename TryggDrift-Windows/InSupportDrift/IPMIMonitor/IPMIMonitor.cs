using InSupport.Drift.Monitor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace InSupport.Drift.Plugins
{
    public class IPMIMonitor : BaseMonitor
    {
        public override string MonitorName => "IPMIMonitor";

        public override float MonitorVersion => 1;

        public IPMIMonitor()
            : base()
        {


        }

        public override void LoadSettings(Dictionary<string, string> settings)
        {
        }

        public IPMISummary Summary
        {
            get
            {
                string path = Path.GetDirectoryName(typeof(IPMIMonitor).Assembly.Location);
                string ipmicfgPath = Path.Combine(path, @"IPMICFG\IPMICFG-Win.exe");

                if (File.Exists(ipmicfgPath))
                {
                    //var summary = TestData("summary.txt").Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    var summary = CmdCommand(ipmicfgPath, "-summary").Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

                    var ip = summary[2].Split(':')[1].Trim();
                    var firmware = summary[4].Split(':')[1].Trim();
                    var bios = summary[6].Split(':')[1].Trim();
                    return new IPMISummary()
                    {
                        IP = ip,
                        Firmware = firmware,
                        Bios = bios
                    };
                }
                return new IPMISummary();
            }
        }

        public IPMIPSU[] PSUInfo
        {
            get
            {
                string path = Path.GetDirectoryName(typeof(IPMIMonitor).Assembly.Location);
                string ipmicfgPath = Path.Combine(path, @"IPMICFG\IPMICFG-Win.exe");

                if (File.Exists(ipmicfgPath))
                {
                    var infoLines = CmdCommand(ipmicfgPath, "-pminfo").Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    //var infoLines = TestData("pminfo.txt").Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    infoLines.RemoveAll(line => string.IsNullOrWhiteSpace(line));

                    var psus = new IPMIPSU[infoLines.Count / 19];


                    for (int i = 0; i < psus.Length; i++)
                    {
                        var status = new Regex(@"(?<=\[)(.*?)(?=\])").Match(infoLines[(i * 19) + 3].Split('|')[1].Trim()).Value;
                        var model = infoLines[(i * 19) + 16].Split('|')[1].Trim();
                        psus[i] = new IPMIPSU()
                        {
                            Status = status,
                            Model = model
                        };
                    }
                    return psus;
                }

                return new IPMIPSU[0];
            }
        }

        public IPMISEL[] EventLog
        {
            get
            {
                try
                {
                    string path = Path.GetDirectoryName(typeof(IPMIMonitor).Assembly.Location);
                    string ipmicfgPath = Path.Combine(path, @"IPMICFG\IPMICFG-Win.exe");

                    if (File.Exists(ipmicfgPath))
                    {
                        //var eventLog = TestData("sel.txt").Replace("\r\n", "") + "|";
                        var eventLog = CmdCommand(ipmicfgPath, "-sel list").Replace("\r\n", "") + "|";
                        var regex = new Regex(@"(?<=\|)(.*?)(?=\|)");
                        var matches = regex.Matches(eventLog);
                        List<IPMISEL> log = new List<IPMISEL>();

                        for (int i = 0; i < matches.Count; i += 3)
                        {
                            var date = matches[i].Value.Trim();
                            var category = matches[i + 1].Value.Trim();
                            var assertion = matches[i + 2].Value.Replace("Assertion:", "").Trim();

                            regex = new Regex(@"(^.*)[^\s\d+$]");
                            assertion = regex.Match(assertion).Value;

                            log.Add(new IPMISEL()
                            {
                                Date = date,
                                Category = category,
                                Assertion = assertion
                            });
                        }

                        log.RemoveAll(l => DateTime.Parse(l.Date) <= DateTime.Now.AddDays(-1));

                        return log.ToArray();
                    }
                    else
                    {
                        return new IPMISEL[]
                        {
                            new IPMISEL() { Assertion = "IPMICFG.exe not found" }
                        };
                    }
                }
                catch (Exception ex)
                {
                    return new IPMISEL[]
                    {
                        new IPMISEL(){ Category = "Exception", Assertion = ex.Message }
                    };
                }
            }
        }

        private string TestData(string file)
        {
            return File.ReadAllText(file);
        }

        //private async Task<string> CmdCommandTask(string command, string args)
        //{
        //    Process p = new Process();

        //    p.StartInfo.UseShellExecute = false;
        //    p.StartInfo.RedirectStandardOutput = true;
        //    p.StartInfo.FileName = command;
        //    p.StartInfo.Arguments = args;
        //    p.Start();

        //    string output = p.StandardOutput.ReadToEnd();
        //    p.WaitForExit();
        //    return output;
        //}

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
    }

    public class IPMISEL
    {
        public string Date { get; set; }
        public string Category { get; set; }
        public string Assertion { get; set; }

        public override string ToString() => $"[{Date}] {Category} - {Assertion}";
    }

    public class IPMIPSU
    {
        public string Status { get; set; }
        public string Model { get; set; }
    }
    public class IPMISummary
    {
        public string IP { get; set; }
        public string Firmware { get; set; }
        public string Bios { get; set; }
    }
}
