using ESLogin.Background;
using LoginShared;
using LoginShared.Administration;
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

namespace ESLogin.Admin
{
    public partial class AddGroupForm : Form
    {
        public AddGroupForm(List<User> users)
        {
            Init(users);
        }

        public AddGroupForm(User currentUser, List<User> users)
        {
            Init(users, currentUser);

            var myUsr = groupsComboBox.Items.Cast<PrettyPrincipal>().Where(pu => pu.SID == currentUser.SID).FirstOrDefault();

            if (myUsr != null)
                groupsComboBox.SelectedItem = myUsr;
            groupsComboBox.Enabled = false;
            authenticatorComboBox.SelectedItem = currentUser.Authenticator;

            foreach (var assignedRole in currentUser.Roles)
            {
                for (int i = 0; i < rolesCheckListBox.Items.Count; i++)
                {
                    var role = (PrettyConfigItem)rolesCheckListBox.Items[i];

                    if (role.ConfigItem.Path == assignedRole)
                    {
                        rolesCheckListBox.SetItemChecked(i, true);
                    }
                }
            }
        }

        private void Init(List<User> existingUsers, User currentUser = null)
        {
            InitializeComponent();

            using (PrincipalContext ctx = Helper.GetPrincipalContext())
            {
                if (currentUser != null)
                {
                    var usr = GroupPrincipal.FindByIdentity(ctx, IdentityType.Sid, currentUser.SID);
                    if (usr != null)
                    {
                        groupsComboBox.Items.Add(new PrettyPrincipal(usr));
                    }
                }
                else
                {
                    using (PrincipalSearcher srch = new PrincipalSearcher(new GroupPrincipal(ctx)))
                    {
                        foreach (var found in srch.FindAll().OrderBy(g => g.Name))
                        {

                            if (found is GroupPrincipal)
                                if (existingUsers.FirstOrDefault(u => u.SID == found.Sid.Value) == null)
                                    groupsComboBox.Items.Add(new PrettyPrincipal(found) { IsGroup = true });
                        }
                    }
                }

                if (groupsComboBox.Items.Count > 0)
                    groupsComboBox.SelectedIndex = 0;
            }

            foreach (var value in Enum.GetValues(typeof(Constants.Authenticators)).Cast<Constants.Authenticators>())
            {
                authenticatorComboBox.Items.Add(value);
            }
            authenticatorComboBox.SelectedIndex = 0;

            foreach (var role in ConfigApiWrapper.GetRoles().OrderBy(r => r.DisplayName))
            {
                rolesCheckListBox.Items.Add(new PrettyConfigItem(role), false);
            }
        }

        private void AddGroupForm_Load(object sender, EventArgs e)
        {

        }

        public User GetGroup()
        {
            var grp = groupsComboBox.SelectedItem as PrettyPrincipal;
            if (grp == null)
            {
                MessageBox.Show("Unable to find existing user. Please delete this user and create a new one", "Error");
                return null;
            }

            return new User()
            {
                DisplayName = grp.Name,
                SID = grp.SID,
                IsGroup = true,
                Authenticator = (Constants.Authenticators)authenticatorComboBox.SelectedItem,
                Roles = GetSelectedRoles()
            };
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

        private void okBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }

    class PrettyPrincipal
    {
        public string SID { get; set; }
        public string Name { get; set; }
        
        public bool IsGroup { get; set; }

        public PrettyPrincipal(Principal user)
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
