using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TryggRetail.Actions
{
    [Serializable]
    class PlayVapixClip_Action : CustomAction
    {
        private string url;
        public PlayVapixClip_Action(string url) : base()
        {
            this.url = url;
            //this.displayName = display;
        }

        public override void Execute()
        {
            try
            {
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadString(url);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
