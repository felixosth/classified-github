using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace REIDShared
{
    public static class Settings
    {

        public static Guid ClientSettingsID = new Guid("{B6707F71-FC06-4002-B19B-B58BD6C94F52}");

        public static string GetNodeREDUrl()
        {
            System.Xml.XmlNode result = VideoOS.Platform.Configuration.Instance.GetOptionsConfiguration(REIDShared.Settings.ClientSettingsID, false);
            if (result != null)
            {
                foreach (XmlNode node in result.ChildNodes)
                {
                    switch (node.Attributes["name"].Value)
                    {
                        
                        case "NodeREDUrl":
                            return node.Attributes["value"].Value;
                    }
                }
            }
            return null;
        }

    }
}
