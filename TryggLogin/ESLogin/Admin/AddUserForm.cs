using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.DirectoryServices.AccountManagement;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ESLogin.Authenticators;
using ESLogin.Background;
using LoginShared;
using LoginShared.Administration;
using VideoOS.ConfigurationAPI;
using YubicoDotNetClient;

namespace ESLogin.Admin
{
    public partial class AddUserForm : Form
    {
        bool cancelClick = false;

        public AddUserForm(List<User> existingUsers)
        {
            Init(existingUsers);

            if (userComboBox.Items.Count > 0)
                userComboBox.SelectedIndex = 0;
        }

        public AddUserForm(User user, List<User> existingUsers)
        {
            Init(existingUsers, user);
            
            var myUsr = userComboBox.Items.Cast<PrettyUser>().Where(pu => pu.SID == user.SID).FirstOrDefault();

            if(myUsr != null)
                userComboBox.SelectedItem = myUsr;

            userComboBox.Enabled = false;

            authenticatorComboBox.SelectedItem = user.Authenticator;
            authDataTxtBox.Text = user.AuthData;
            
            foreach(var assignedRole in user.Roles)
            {
                for (int i = 0; i < rolesCheckListBox.Items.Count; i++)
                {
                    var role = (PrettyConfigItem)rolesCheckListBox.Items[i];

                    if(role.ConfigItem.Path == assignedRole)
                    {
                        rolesCheckListBox.SetItemChecked(i, true);
                    }
                }
            }
        }

        void Init(List<User> existingUsers, User currentUser = null)
        {
            InitializeComponent();

            authDataTxtBox.PreviewKeyDown += (s, e) =>
            {
                if(e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
                {
                    cancelClick = true;
                }
            };

            foreach (var value in Enum.GetValues(typeof(Constants.Authenticators)).Cast<Constants.Authenticators>())
            {
                authenticatorComboBox.Items.Add(value);
            }

            authenticatorComboBox.SelectedIndex = 0;

            foreach (var role in ConfigApiWrapper.GetRoles().OrderBy(r => r.DisplayName))
            {
                rolesCheckListBox.Items.Add(new PrettyConfigItem(role), false);
            }

            using (var context = Helper.GetPrincipalContext())
            {
                if (currentUser != null)
                {
                    var user = UserPrincipal.FindByIdentity(context, IdentityType.Sid, currentUser.SID);
                    if(user != null)
                    {
                        userComboBox.Items.Add(new PrettyUser(user));
                    }
                }
                else
                    using (var searcher = new PrincipalSearcher(new UserPrincipal(context)))
                    {
                        //searcher.QueryFilter = new UserPrincipal(context) { Name = "*" };
                        foreach (var result in searcher.FindAll().OrderBy(r => r.Name))
                        {
                            if (result is UserPrincipal)
                                if (existingUsers.FirstOrDefault(u => u.SID == result.Sid.Value) == null)
                                userComboBox.Items.Add(new PrettyUser(result));
                        }
                    }
            }
        }

        private async void okBtn_Click(object sender, EventArgs e)
        {
            if (cancelClick)
            {
                cancelClick = false;
                return;
            }

            if ((Constants.Authenticators)authenticatorComboBox.SelectedItem == Constants.Authenticators.Yubikey && authDataTxtBox.Text.Length > 12)
            {
                IYubicoResponse result = null;
                try
                {
                    result = await YubikeyAuthenticator.TestYubikey(authDataTxtBox.Text);
                }
                catch(FormatException)
                {
                    MessageBox.Show("Invalid YubiKey OTP format.");
                }

                if(result.Status != YubicoDotNetClient.YubicoResponseStatus.Ok)
                {
                    MessageBox.Show("Error verifying YubiKey: " + result.Status);
                    return;
                }

                authDataTxtBox.Text = result.PublicId;
            }

            DialogResult = DialogResult.OK;
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private string[] GetSelectedRoles()
        {
            List<string> roles = new List<string>();
            foreach (PrettyConfigItem item in rolesCheckListBox.CheckedItems)
            {
                roles.Add(item.ConfigItem.Path);
            }
            return roles.ToArray();
        }

        public User GetUser()
        {
            var user = userComboBox.SelectedItem as PrettyUser;

            if(user == null)
            {
                MessageBox.Show("Unable to find existing user. Please delete this user and create a new one", "Error");
                return null;
            }

            return new User()
            {
                DisplayName = user.Name,
                SID = user.SID,
                AuthData = authDataTxtBox.Text,
                Authenticator = (Constants.Authenticators)authenticatorComboBox.SelectedItem,
                Roles = GetSelectedRoles()
            };
        }

        private void authenticatorComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch((Constants.Authenticators)authenticatorComboBox.SelectedItem)
            {
                case Constants.Authenticators.Yubikey:
                    authDataLabel.Text = "YubiKey OTP";
                    //authDataLabel.Text = "Yubikey not licensed.";
                    // DISABLED
                    //authDataTxtBox.Enabled = false;
                    //okBtn.Enabled = false;
                    break;
                case Constants.Authenticators.BankID:
                    //okBtn.Enabled = true;
                    //authDataTxtBox.Enabled = true;

                    authDataLabel.Text = "Personal number";
                    break;
            }
        }

        List<Keys> keys = new List<Keys>();

        private void authDataTxtBox_KeyDown(object sender, KeyEventArgs e)
        {
            keys.Add(e.KeyCode);
            if (e.KeyCode == Keys.Return)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }
    }

    public class PrettyConfigItem
    {
        public ConfigurationItem ConfigItem { get; set; }
        public PrettyConfigItem(ConfigurationItem item)
        {
            this.ConfigItem = item;
        }

        public override string ToString()
        {
            return ConfigItem.DisplayName;
        }
    }

    public class PrettyUser
    {
        public string SID { get; set; }
        public string Name { get; set; }

        public PrettyUser(Principal user)
        {
            this.SID = user.Sid.Value;
            this.Name = user.Name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
