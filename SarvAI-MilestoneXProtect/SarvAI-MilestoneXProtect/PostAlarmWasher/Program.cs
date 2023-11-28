using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SarvAI.Milestone.Verifier;
using VideoOS.Platform;
using VideoOS.Platform.Proxy.Alarm;
using VideoOS.Platform.Data;
using System.IO;
using System.Reflection;

namespace PostAlarmWasher
{
    class Program
    {
        static void Main(string[] rawArgs)
        {
            string videoDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Videos");
            var args = ParseArgs(rawArgs);

            DateTime from = DateTime.UtcNow.AddDays(-2);
            DateTime to = DateTime.UtcNow;

            bool saveVideos = args.ContainsKey("saveVideos") ? bool.Parse(args["saveVideos"]) : false;

            if (args.ContainsKey("from"))
                from = DateTime.Parse(args["from"]).ToUniversalTime();

            if (args.ContainsKey("to"))
                to = DateTime.Parse(args["to"]).ToUniversalTime();

            int prePostTime = args.ContainsKey("prepost") ? int.Parse(args["prepost"]) : 3000;

            AlarmExporter alarmExporter = new AlarmExporter(null, false, preAndPostTimeMs: prePostTime);

            var alarmClient = alarmExporter.GetAlarmClient(EnvironmentManager.Instance.MasterSite);

            var conditions = new List<Condition>()
            {
                new Condition()
                {
                    Operator = Operator.GreaterThan,
                    Target = Target.Timestamp,
                    Value = from
                },
                new Condition()
                {
                    Operator = Operator.LessThan,
                    Target = Target.Timestamp,
                    Value = to
                }
            };

            if(args.ContainsKey("priority"))
            {
                conditions.Add(new Condition()
                {
                    Operator = Operator.Equals,
                    Target = Target.Priority,
                    Value = int.Parse(args["priority"])
                });
            }

            if (args.ContainsKey("state"))
            {
                conditions.Add(new Condition()
                {
                    Operator = Operator.Equals,
                    Target = Target.State,
                    Value = int.Parse(args["state"])
                });
            }

            var alarms = alarmClient.GetAlarmLines(0, 500, new AlarmFilter
            {
                Conditions = conditions.ToArray()
            });

            if(saveVideos)
            {
                Directory.CreateDirectory(videoDir);
            }

            Console.WriteLine("\"date\",\"alarmId\",\"alarm\",\"camera\",\"result\",\"score\"");


            foreach (var alarm in alarms)
            {
                var cam = Configuration.Instance.GetItem(alarm.CameraId, Kind.Camera);
                FQID fqidToExport = cam?.FQID;


                if(fqidToExport != null)
                {
                    //Console.WriteLine("Processing alarm {0}: {1}, camera {2} from {3}", alarm.LocalId, alarm.Message, cam.Name, alarm.Timestamp);

                    try
                    {
                        var result =
                            alarmExporter.ProcessAlarm(
                                fqidToExport,
                                alarm: new Alarm()
                                {
                                    EventHeader = new EventHeader()
                                    {
                                        ID = alarm.Id,
                                        Timestamp = alarm.Timestamp,
                                        Source = new EventSource()
                                        {
                                            FQID = fqidToExport
                                        }
                                    }
                                },
                                isPostProcess: true,
                                deleteVideoAfter: !saveVideos);



                        if (result == null)
                        {
                            //Console.WriteLine("Washing failed");
                            Console.WriteLine("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\"", alarm.Timestamp.ToLocalTime(), alarm.LocalId, alarm.Message, cam.Name, "Failed", 0);

                        }
                        else
                        {
                            Console.WriteLine("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\"", alarm.Timestamp.ToLocalTime(), alarm.LocalId, alarm.Message, cam.Name, result.Event, result.Score);

                            if (saveVideos)
                            {
                                File.Move(result.LocalVideoFile, Path.Combine(videoDir, MakeSafeFilename($"{alarm.Timestamp.ToLocalTime()}, {cam.Name}, {result.Event}, {result.Score}.mkv", '_')));
                            }
                        }
                    }
                    catch
                    {
                        Console.WriteLine("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\"", alarm.Timestamp.ToLocalTime(), alarm.LocalId, alarm.Message, cam.Name, "Error", 0);
                    }

                    //Console.WriteLine("Result: {0} ({1})", result.Event, result.Score);

                    //Console.WriteLine();

                }


            }

            //Console.ReadKey();
        }

        public static string MakeSafeFilename(string filename, char replaceChar)
        {
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                filename = filename.Replace(c, replaceChar);
            }
            return filename;
        }

        static Dictionary<string, string> ParseArgs(string[] args)
        {
            Dictionary<string, string> parsed = new Dictionary<string, string>();

            foreach (var arg in args)
            {
                var split = arg.Split('=');
                if (split.Length == 2)
                {
                    parsed.Add(split[0], split[1]);
                }
            }
            return parsed;
        }
    }
}
