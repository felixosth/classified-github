using ESLogin;
using LoginShared;
using LoginShared.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VideoOS.Platform;
using VideoOS.Platform.Admin;

namespace ESLogin.Admin
{
    public class ESLoginItemManager : ItemManager
    {
        public static Guid ConfigItemId = new Guid("{88F21EB5-51DE-455B-BC08-43FEFD6C9B2B}");

        AdminUserControl userControl;

        Messaging messaging = new Messaging();

        Item configItem;

        private Guid _kind;
        public ESLoginItemManager(Guid kind)
        {
            _kind = kind;
        }

        public override void Init()
        {
            if (EnvironmentManager.Instance.EnvironmentType == EnvironmentType.Administration)
            {
                configItem = Configuration.Instance.GetItemConfiguration(ESLoginDefinition.ESLoginPluginId, _kind, ConfigItemId);

                if (configItem == null)
                {
                    configItem = new Item(GenerateFQID(ConfigItemId), "Configuration");
                    Configuration.Instance.SaveItemConfiguration(ESLoginDefinition.ESLoginPluginId, configItem);
                }
            }
        }

        bool haveInit = false;
        public override UserControl GenerateDetailUserControl()
        {
            if (!haveInit)
            {
                messaging.Init(ignoreWait: true);
                haveInit = true;
            }

            userControl = new AdminUserControl();
            userControl.ConfigurationChanged += ConfigurationChangedByUserHandler;
            return userControl;
        }

        public override void ReleaseUserControl()
        {
            if (userControl != null)
            {
                userControl.ConfigurationChanged -= ConfigurationChangedByUserHandler;
                userControl = null;
            }
        }

        public override void FillUserControl(Item item)
        {
            if (userControl != null && configItem != null)
                userControl.FillContent(configItem);
        }

        //public override void ClearUserControl()
        //{
        //    //base.ClearUserControl();
        //}

        public override bool ValidateAndSaveUserControl()
        {
            if(userControl != null)
            {
                userControl.UpdateItem(configItem);
            }

            Configuration.Instance.SaveItemConfiguration(ESLoginDefinition.ESLoginPluginId, configItem);
            messaging.Transmit(Constants.MessageID, new MessageData(Constants.Actions.ConfigurationChanged));
            return true;
        }


        public override bool IsContextMenuValid(string command)
        {
            return false;
        }

        FQID GenerateFQID(Guid id) => new FQID(EnvironmentManager.Instance.MasterSite.ServerId, Guid.Empty, id, FolderType.UserDefined, _kind);
    }
}
