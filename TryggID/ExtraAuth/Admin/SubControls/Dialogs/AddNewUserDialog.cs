using AuthenticationSystem;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VideoOS.ConfigurationAPI;

namespace TryggLogin.Admin.SubControls.Dialogs
{
    public partial class AddNewUserDialog : Form
    {
        private class PrettyConfigurationItem
        {
            public ConfigurationItem ConfigurationItem { get; set; }
            public PrettyConfigurationItem(ConfigurationItem item)
            {
                this.ConfigurationItem = item;
            }

            public override string ToString() => ConfigurationItem.DisplayName;
        }

        public AddNewUserDialog()
        {
            InitializeComponent();

            var roles = new AuthenticationSystem.UserManagement().GetRoles();
            foreach(var role in roles)
            {
                roleComboBox.Items.Add(new PrettyConfigurationItem(role));
            }
            roleComboBox.SelectedIndex = 0;

            keyTypeComboBox.DataSource = Enum.GetValues(typeof(AuthKeyTypes));
            keyTypeComboBox.SelectedIndex = 0;
        }

        public TryggLoginUser EditUser(TryggLoginUser user)
        {
            displayNameTxtBox.Text = user.DisplayName;
            idKeyTxtBox.Text = user.Key;
            keyTypeComboBox.SelectedIndex = (int)user.KeyType;

            foreach(PrettyConfigurationItem item in roleComboBox.Items)
            {
                if(item.ConfigurationItem.Path == user.BelongingRolePath)
                {
                    roleComboBox.SelectedItem = item;
                    break;
                }
            }


            if(this.ShowDialog() == DialogResult.OK)
            {
                return new TryggLoginUser(displayNameTxtBox.Text, idKeyTxtBox.Text, (roleComboBox.SelectedItem as PrettyConfigurationItem).ConfigurationItem.Path, passwordTxtBox.Text)
                {
                    RoleName = (roleComboBox.SelectedItem as PrettyConfigurationItem).ConfigurationItem.DisplayName,
                    KeyType = (AuthKeyTypes)keyTypeComboBox.SelectedIndex
                };
            }
            return null;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(ValidateForm())
            {
                DialogResult = DialogResult.OK;
            }

        }

        private bool ValidateForm()
        {
            string msg = "";
            if (displayNameTxtBox.Text == "")
                msg += "Please fill in a Display Name";


            if (msg != "")
            {
                MessageBox.Show(msg, "Invalid input", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return false;
            }
            return true;
        }

        public TryggLoginUser ResultUser()
        {
            return new TryggLoginUser(displayNameTxtBox.Text, idKeyTxtBox.Text, (roleComboBox.SelectedItem as PrettyConfigurationItem).ConfigurationItem.Path, passwordTxtBox.Text)
            {
                RoleName = (roleComboBox.SelectedItem as PrettyConfigurationItem).ConfigurationItem.DisplayName,
                KeyType = (AuthKeyTypes)keyTypeComboBox.SelectedIndex
            };
        }
    }
}
