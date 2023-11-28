using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TryggRetail.Admin;
using VideoOS.Platform;
using VideoOS.Platform.Admin;

namespace TryggRetail_Configurator
{
    public partial class ConfigForm : Form
    {
        TryggRetailUserControl configurator = new TryggRetailUserControl();
        Item currentItem;
        internal static Guid TryggRetailPluginID = new Guid("d0677ba9-3bdc-451a-9088-ea4e7154af70");
        internal Guid TryggRetailKind = new Guid("6d40025c-adad-4d82-aecc-bd591887252f");
        internal const string LicenseID = "InSupport-TryggRetail-ConcurrentLicense";

        public ConfigForm()
        {
            InitializeComponent();
        }

        private void ConfigForm_Load(object sender, EventArgs e)
        {
            RefreshList();
        }

        internal void RefreshList(int selectIndex = 0)
        {
            SavePrompt();
            configurator.ClearContent();
            itemsListBox.Items.Clear();
            var items = Configuration.Instance.GetItemsByKind(TryggRetailPluginID);
            Configuration.Instance.RefreshConfiguration(TryggRetailPluginID);
            var configItems = Configuration.Instance.GetItemConfigurations(TryggRetailPluginID, null, TryggRetailKind);

            foreach (var item in configItems)
            {
                itemsListBox.Items.Add(item);
            }

            panel1.Controls.Add(configurator);
            if (itemsListBox.Items.Count > 0)
                itemsListBox.SelectedIndex = selectIndex;

            somethingChanged = false;
        }

        bool somethingChanged = false;
        private void itemsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SavePrompt();

            configurator.ClearContent();
            currentItem = itemsListBox.SelectedItem as Item;
            if (currentItem == null)
                return;
            configurator.FillContent(currentItem);
            somethingChanged = false;
            configurator.ConfigurationChangedByUser += Configurator_ConfigurationChangedByUser;
        }

        internal void SavePrompt()
        {
            if (somethingChanged)
            {
                if (MessageBox.Show("Something changed. Save?", "TryggRetail", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    UpdateAndSave();
                    somethingChanged = false;
                }
                //RefreshList(itemsListBox.SelectedIndex);
            }
        }

        internal void UpdateAndSave()
        {
            configurator.UpdateItem(currentItem);
            Configuration.Instance.SaveItemConfiguration(TryggRetailPluginID, currentItem);
        }

        private void Configurator_ConfigurationChangedByUser(object sender, EventArgs e)
        {
            somethingChanged = true;
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            if (currentItem == null)
                return;

            UpdateAndSave();
        }

        private void refreshListBtn_Click(object sender, EventArgs e)
        {
            RefreshList(-1);
        }

        private void deleteItemBtn_Click(object sender, EventArgs e)
        {
            if (currentItem == null)
                return;

            if(MessageBox.Show("Are you sure you want to delete this item?", "TryggRetail deletion", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Configuration.Instance.DeleteItemConfiguration(TryggRetailPluginID, currentItem);
                RefreshList(-1);
            }
        }

        private void newItemBtn_Click(object sender, EventArgs e)
        {
            var fqid = new FQID();
            fqid.Kind = TryggRetailKind;
            fqid.ServerId = Configuration.Instance.ServerFQID.ServerId;
            fqid.ObjectId = Guid.NewGuid();

            var newItem = new Item(fqid, "New TryggRetail Item");
            Configuration.Instance.SaveItemConfiguration(TryggRetailPluginID, newItem);

            RefreshList(-1);
        }

        private void licBtn_Click(object sender, EventArgs e)
        {
            //SavePrompt();
            new LicenseManagementForm().Show();
        }

        private void ConfigForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SavePrompt();
        }
    }
}
