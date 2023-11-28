using REIDSearchAgent.SearchAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using VideoOS.Platform.Client;

namespace REIDSearchAgent.Settings
{
    public class ReidSettingsPlugin : SettingsPanelPlugin
    {
        internal const string NodeREDUrl = "NodeREDUrl";

        SettingsUserControl userControl;

        public override Guid Id => REIDShared.Settings.ClientSettingsID;

        public override string Title => "Reid settings";

        public override void Close()
        {
        }

        public override void CloseUserControl()
        {
            userControl = null;
        }

        public override UserControl GenerateUserControl()
        {
            userControl = new SettingsUserControl(this);
            return userControl;
        }

        public override void Init()
        {
            LoadProperties(false); // Builtin method
        }

        public override bool TrySaveChanges(out string errorMessage)
        {
            errorMessage = string.Empty;
            Uri uri = null;
            if (Uri.TryCreate(userControl.NodeREDUrl, UriKind.RelativeOrAbsolute, out uri))
            {
                ReidPluginSearchAgentDefinition.NodeRED = new REIDShared.NodeREDConnection(uri);
                if (ReidPluginSearchAgentDefinition.NodeRED.Connect())
                {
                    SetProperty(NodeREDUrl, userControl.NodeREDUrl);
                    SaveProperties(false);
                }
                else
                {
                    errorMessage = "Unable to connect to NodeRED.";
                    return false;
                }
            }
            else
            {
                errorMessage = "Unable to parse URL.";
                return false;
            }

            return true;
        }
    }
}
