using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VideoOS.Platform;
using TryggLogin.Admin.SubControls;

namespace TryggLogin.Admin
{
    public partial class AdminUserControl : UserControl
    {
        internal event EventHandler ConfigurationChangedByUser;
        private string itemName;
        internal string DisplayName  { get{return itemName;}
            set
            {
                itemName = value;
                OnUserChange(this, new EventArgs());
            }
        }

        //LicenseUserControl licUsrCtrl;
        //UsrMgmntUserControl usrMgmntCtrl;
        BaseSubUserControl activeControl;


        public AdminUserControl()
        {
            InitializeComponent();

            this.Dock = DockStyle.Fill;
        }

        internal void OnUserChange(object sender, EventArgs e)
        {
            //ConfigurationChangedByUser?.Invoke(this, new EventArgs());
            if (ConfigurationChangedByUser != null)
                ConfigurationChangedByUser(this, e);
        }

        internal void FillContent(Item item)
        {
            ClearContent();
            itemName = item.Name;

            if (item.Name == "License")
            {
                activeControl = new LicenseUserControl(this);
            }
            else if(item.Name == "Users")
            {
                activeControl = new UsrMgmntUserControl(this);
            }

            if (activeControl != null)
            {
                this.Controls.Add(activeControl);
                activeControl.Dock = DockStyle.Fill;
                activeControl.Fill(item);
            }
        }

        internal void UpdateItem(Item item)
        {
            item.Name = itemName;
            activeControl?.UpdateItem(item);
        }

        internal void ClearContent()
        {
            this.Controls.Clear();
            activeControl?.Dispose();
            activeControl = null;
        }

    }
}
