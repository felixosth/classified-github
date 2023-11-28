using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoOS.Platform.Client;

namespace TryggRetail.PopupWindow.Live
{
    public class LiveViewManager : ViewItemPlugin
    {
        //private static System.Drawing.Image _treeNodeImage;

        Background.TryggRetailBackgroundPlugin instance;
        bool liveMode;

        public LiveViewManager(Background.TryggRetailBackgroundPlugin _instance, bool liveMode = false)
        {
            this.instance = _instance;
            this.liveMode = liveMode;
            //Assembly assembly = Assembly.GetExecutingAssembly();
            //string name = assembly.GetName().Name;
            //_treeNodeImage = System.Drawing.Image.FromStream(assembly.GetManifestResourceStream(name + "Resources.WorkSpaceIcon.bmp"));
        }

        public override Guid Id
        {
            get { return TryggRetailDefinition.TryggRetailLiveViewPlugin; }
        }

        public override System.Drawing.Image Icon
        {
            get { return Properties.Resources.WorkSpaceIcon; }
        }

        public override string Name
        {
            get { return "InSupport AlarmPopup Live Player"; }
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
            return new LiveViewItemManager(instance);
        }

        public override void Init()
        {
        }

        public override void Close()
        {
        }


    }


    public class LiveViewItemManager : ViewItemManager
    {
        Background.TryggRetailBackgroundPlugin instance;
        public LiveViewItemManager(Background.TryggRetailBackgroundPlugin _instance) : base("LiveViewItemManager")
        {
            this.instance = _instance;
        }

        public override ViewItemUserControl GenerateViewItemUserControl()
        {
            return new LiveViewUserControl(instance);
        }

        public override PropertiesUserControl GeneratePropertiesUserControl()
        {
            return new PropertiesUserControl(); //no special properties
        }

    }
}
