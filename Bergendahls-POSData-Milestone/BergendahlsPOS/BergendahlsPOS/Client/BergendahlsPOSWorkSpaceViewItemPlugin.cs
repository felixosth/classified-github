using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using VideoOS.Platform.Client;

namespace BergendahlsPOS.Client
{
    public class BergendahlsPOSWorkSpaceViewItemPlugin : ViewItemPlugin
    {
        private static System.Drawing.Image _treeNodeImage;

        public BergendahlsPOSWorkSpaceViewItemPlugin()
        {
            _treeNodeImage = Properties.Resources.WorkSpaceIcon;
        }

        public override Guid Id
        {
            get { return BergendahlsPOSDefinition.BergendahlsPOSWorkSpaceViewItemPluginId; }
        }

        public override System.Drawing.Image Icon
        {
            get { return _treeNodeImage; }
        }

        public override string Name
        {
            get { return "POS Data WorkSpace Plugin View Item"; }
        }

        public override bool HideSetupItem
        {
            get
            {
                return true;
            }
        }

        public override ViewItemManager GenerateViewItemManager()
        {
            return new BergendahlsPOSWorkSpaceViewItemManager();
        }

        public override void Init()
        {
        }

        public override void Close()
        {
        }

    }
}
