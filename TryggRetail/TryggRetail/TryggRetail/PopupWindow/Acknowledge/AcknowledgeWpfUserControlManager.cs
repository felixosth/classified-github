using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoOS.Platform.Client;
using TryggRetail.Background;
using System.Reflection;
using TryggRetail.Admin;

namespace TryggRetail.PopupWindow.Acknowledge
{
    public class AcknowledgeWpfUserControlManager : ViewItemPlugin
    {
        PopupConfig config;
        TryggRetailBackgroundPlugin instance;

        public AcknowledgeWpfUserControlManager(PopupConfig _conf, TryggRetailBackgroundPlugin instance)
        {
            this.instance = instance;
            this.config = _conf;
        }

        public override Guid Id
        {
            get { return TryggRetailDefinition.TryggRetailAcknowledgeUserControlID; }
        }

        public override System.Drawing.Image Icon
        {
            get { return Properties.Resources.WorkSpaceIcon; }
        }

        public override string Name
        {
            get { return "InSupport AlarmPopup"; }
        }

        public override bool HideSetupItem  // Visa i setup skärmen där man kan lägga till plugins i vyer
        {
            get
            {
                return false;
            }
        }

        public override ViewItemManager GenerateViewItemManager()
        {
            return new AcknowledgeUserControlWorkSpaceMng(config, instance);
            //return AckMng;
        }

        public override void Init()
        {
        }

        public override void Close()
        {
        }


    }


    public class AcknowledgeUserControlWorkSpaceMng : ViewItemManager
    {
        PopupConfig config;
        TryggRetailBackgroundPlugin instance;

        public AcknowledgeUserControlWorkSpaceMng(PopupConfig _conf, TryggRetailBackgroundPlugin instance) : base("AcknowledgeUserControlWorkSpaceMng")
        {
            this.instance = instance;
            this.config = _conf;
        }

        public override ViewItemUserControl GenerateViewItemUserControl()
        {
            return new AcknowledgeUserControl(instance);
        }

        //public override ViewItemWpfUserControl GenerateViewItemWpfUserControl()
        //{
        //    return new AcknowledgeWpfUserControl(/*config, */instance);
        //}

        public override PropertiesUserControl GeneratePropertiesUserControl()
        {
            return new PropertiesUserControl(); //no special properties
        }

    }
}
