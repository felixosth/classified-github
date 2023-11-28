using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace InSupport
{
    class UpdateCheck
    {
        public float NewVersion { get;set; }

        public bool NewUpdateReleased(float currentVer)
        {
            try
            {
                using (var webClient = new WebClient())
                {
                    NewVersion = float.Parse(webClient.DownloadString(new Uri("http://83.233.164.117/plugin/version")));
                    if (NewVersion > currentVer)
                        return true;
                }

                return false;
            }
            catch { return false; }
        }
    }
}
