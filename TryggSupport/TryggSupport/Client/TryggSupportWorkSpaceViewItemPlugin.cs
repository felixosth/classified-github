using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using VideoOS.Platform.Client;

namespace TryggSupport.Client
{
    public class TryggSupportWorkSpaceViewItemPlugin : ViewItemPlugin
    {
        private static System.Drawing.Image _treeNodeImage;

        public TryggSupportWorkSpaceViewItemPlugin()
        {
            _treeNodeImage = Properties.Resources.WorkSpaceIcon;
        }

        public override Guid Id
        {
            get { return TryggSupportDefinition.TryggSupportWorkSpaceViewItemPluginId; }
        }

        public override System.Drawing.Image Icon
        {
            get { return _treeNodeImage; }
        }

        public override string Name
        {
            get { return "WorkSpace Plugin View Item"; }
        }

        public override bool HideSetupItem
        {
            get
            {
                return false;
            }
        }

        public override ViewItemManager GenerateViewItemManager()
        {
            return new TryggSupportWorkSpaceViewItemManager();
        }

        public override void Init()
        {
        }

        public override void Close()
        {
        }


    }
}
