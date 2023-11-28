using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebcamBridge
{
    internal static class Config
    {
        internal static Dictionary<string, string> GetConfig()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            string cfgPath = GetPath();

            string[] lines = null;

            if (File.Exists(cfgPath)) // read config
            {
                lines = File.ReadAllLines(cfgPath);
            }
            else // Build default config
            {
                lines = new string[]
                {
                    Constants.ServerBind + "=*",
                    Constants.ServerPort + "=8124",
                    Constants.CameraAddress + "=http://192.168.0.90:80",
                    Constants.CameraUsername + "=root",
                    Constants.CameraPassword + "=pass",
                    Constants.CameraResolution + "=1920x1080",
                    Constants.CameraImageFetchIntervalMs + "=100",
                    Constants.TrustAllCertificates + "=" + bool.FalseString
                };
                File.WriteAllLines(cfgPath, lines);
            }

            foreach (var line in lines)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    var split = line.Split('=');
                    if (split.Length == 2)
                    {
                        var key = split[0];
                        var value = split[1];
                        dict.Add(key, value);
                    }
                }
            }

            return dict;
        }

        private static string GetPath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.cfg");
        }
    }
}
