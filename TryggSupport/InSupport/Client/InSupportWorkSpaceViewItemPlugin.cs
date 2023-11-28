using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using VideoOS.Platform.Client;

namespace InSupport.Client
{
    public class InSupportWorkSpaceViewItemPlugin : ViewItemPlugin
    {
        private static System.Drawing.Image _treeNodeImage;

        public InSupportWorkSpaceViewItemPlugin()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string name = assembly.GetName().Name;
            _treeNodeImage = System.Drawing.Image.FromStream(assembly.GetManifestResourceStream(name + ".Resources.InSupportWorkSpace.bmp"));
        }

        public override Guid Id
        {
            get { return InSupportDefinition.InSupportWorkSpaceViewItemPluginId; }
        }

        public override System.Drawing.Image Icon
        {
            get { return _treeNodeImage; }
        }

        public override string Name
        {
            get { return "InSupport Plugin"; }
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
            return new InSupportWorkSpaceViewItemManager();
        }

        public override void Init()
        {
        }

        public override void Close()
        {
        }


    }
}
