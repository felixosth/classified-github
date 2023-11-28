using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using VideoOS.Platform.Client;

namespace UVAPMipPlugin.Settings
{
    // This is the entry point of the SettingsPanel plugin. This class derives from the Milestone api class SettingsPanelPlugin
    // We create/return the user control here and enables the user to save some settings to disable the overlay plugin
    public class OverlaySettingsPlugin : SettingsPanelPlugin
    {
        OverlaySettingsUserControl userControl;

        internal const string SkeletonKey = "OverlaySkeletonEnabled";
        internal const string HeadKey = "OverlayHeadEnabled";

        internal static Guid ID = new Guid("{2CFF67E9-4C01-4DE1-A8BE-2702340BC7C0}");

        public override Guid Id => ID;

        public override string Title => "UVAP overlay setup";

        public override void Close()
        {
        }

        public override void CloseUserControl()
        {
            userControl = null;
        }

        public override void Init()
        {
            LoadProperties(true); // Builtin method
        }

        public override UserControl GenerateUserControl()
        {
            userControl = new OverlaySettingsUserControl(this); // The settings are set by this user control
            return userControl;
        }

        public override bool TrySaveChanges(out string errorMessage)
        {
            SaveProperties(true); // Buildin method
            errorMessage = string.Empty;
            return true;
        }
    }
}
