using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VideoOS.Platform.Client;

namespace TryggRetail.Playback
{
    public class PlaybackAlarmManager : ViewItemPlugin
    {
        //private static System.Drawing.Image _treeNodeImage;

        Background.TryggRetailBackgroundPlugin instance;
        bool liveMode;

        public PlaybackAlarmManager(Background.TryggRetailBackgroundPlugin _instance, bool liveMode = false)
        {
            this.instance = _instance;
            this.liveMode = liveMode;
            //Assembly assembly = Assembly.GetExecutingAssembly();
            //string name = assembly.GetName().Name;
            //_treeNodeImage = System.Drawing.Image.FromStream(assembly.GetManifestResourceStream(name + "Resources.WorkSpaceIcon.bmp"));
        }

        public override Guid Id
        {
            get { return TryggRetailDefinition.TryggRetailWorkSpaceViewItemPluginId; }
        }

        public override System.Drawing.Image Icon
        {
            get { return Properties.Resources.WorkSpaceIcon; }
        }

        public override string Name
        {
            get { return "InSupport AlarmPopup Playback player"; }
        }

        public override bool HideSetupItem  // Visa i setup skärmen där man kan lägga till plugins i vyer
        {
            get
            {
                return true;
            }
        }

        public override ViewItemManager GenerateViewItemManager()
        {
            return new AlarmWorkSpaceViewItemManager(instance, liveMode);
        }

        public override void Init()
        {
        }

        public override void Close()
        {
        }


    }


    public class AlarmWorkSpaceViewItemManager : ViewItemManager
    {
        Background.TryggRetailBackgroundPlugin instance;
        bool liveMode;
        public AlarmWorkSpaceViewItemManager(Background.TryggRetailBackgroundPlugin _instance, bool liveMode) : base("InSupportWorkSpaceViewItemManager")
        {
            this.instance = _instance;
            this.liveMode = liveMode;
        }

        public override ViewItemUserControl GenerateViewItemUserControl()
        {
            return new PlaybackAlarmUserControl(instance, liveMode);
        }

        public override PropertiesUserControl GeneratePropertiesUserControl()
        {
            return new PropertiesUserControl(); //no special properties
        }

    }
}
