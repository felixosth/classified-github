using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InSupport
{
    public static class Cfg
    {
        public static Dictionary<string, string> Parse(string cfgPath)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            string[] lines;

            if (!File.Exists(cfgPath))
            {
                lines = new string[]
                {
                    "Name=" + Environment.MachineName,
                    "Interval=600",
                    "DriftUrl=https://drift.tryggconnect.se"
                };

                Directory.CreateDirectory(Path.GetDirectoryName(cfgPath));

                File.WriteAllLines(cfgPath, lines);
            }
            else
                lines = File.ReadAllLines(cfgPath);

            foreach (var line in lines)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    var split = line.Split('=');
                    if (split.Length == 2)
                    {
                        var key = split[0];
                        var value = string.Join("", split.Skip(1)); // split[1];
                        dict.Add(key, value);
                    }
                }
            }

            return dict;
        }

        public static string GetPath(int instanceId = -1)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;

            string instance = instanceId > -1 ? $"-{instanceId}" : "";
            path = Path.Combine(path, $"config{instance}.cfg");

            return path;
        }
    }
}
