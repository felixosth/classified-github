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
using LoginShared.Administration;
using LoginShared;

namespace ESLogin.Admin
{
    public partial class AdminUserControl : UserControl
    {
        public event EventHandler ConfigurationChanged;

        Item cfgItem;
        List<User> users;

        public AdminUserControl()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
        }

        public void FillContent(Item item)
        {
            this.cfgItem = item;
            if (cfgItem.Properties.ContainsKey(Constants.UsersConfigKey))
            {
                var usersRaw = cfgItem.Properties[Constants.UsersConfigKey];

                try
                {
                    users = Packer.Deserialize<List<User>>(usersRaw);
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error reading configuration\r\n\r\n" + ex.ToString());
                    users = new List<User>();
                }
            }
            else
            {
                users = new List<User>();
            }

            RefreshUsersTable();
        }

        void RefreshUsersTable()
        {
            usersListView.Items.Clear();

            foreach(var user in users.OrderBy(u => u.DisplayName))
            {
                var item = new ListViewItem(user.DisplayName);
                item.SubItems.Add(user.Authenticator.ToString());
                item.SubItems.Add(user.IsGroup ? "Group" : "User");
                item.Tag = user;

                usersListView.Items.Add(item);
            }
        }

        internal void UpdateItem(Item item)
        {
            cfgItem.Properties[Constants.UsersConfigKey] = Packer.Serialize(users);
            item = cfgItem;
        }

        private void ConfigChanged(object sender, EventArgs e)
        {
            ConfigurationChanged?.Invoke(sender, e);
        }

        private void usersListViewContextMenu_Opening(object sender, CancelEventArgs e)
        {
            removeUserToolStripMenuItem.Enabled = usersListView.SelectedItems.Count != 0;
            editUserToolStripMenuItem.Enabled = usersListView.SelectedItems.Count != 0;
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var addUserForm = new AddUserForm(users) { Owner = this.FindForm() })
            {
                if (addUserForm.ShowDialog() == DialogResult.OK)
                {
                    var newUsr = addUserForm.GetUser();
                    if(newUsr != null)
                    {
                        users.Add(newUsr);
                        RefreshUsersTable();
                        ConfigChanged(this, new EventArgs());
                    }
                }
            }
        }

        private void editUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectedUser = usersListView.SelectedItems[0].Tag as User;

            if (!selectedUser.IsGroup)
                using (var addUserForm = new AddUserForm(selectedUser, users) { Owner = this.FindForm() })
                {
                    if (addUserForm.ShowDialog() == DialogResult.OK)
                    {
                        var modifiedUsr = addUserForm.GetUser();
                        if (modifiedUsr != null)
                        {
                            users[users.IndexOf(selectedUser)] = modifiedUsr;
                            RefreshUsersTable();
                            ConfigChanged(this, new EventArgs());
                        }
                    }
                }
            else
                using (var addGroupForm = new AddGroupForm(selectedUser, users) { Owner = this.FindForm() })
                {
                    if (addGroupForm.ShowDialog() == DialogResult.OK)
                    {
                        var newGrp = addGroupForm.GetGroup();
                        if (newGrp != null)
                        {
                            users[users.IndexOf(selectedUser)] = newGrp;
                            RefreshUsersTable();
                            ConfigChanged(this, new EventArgs());
                        }
                    }
                }
        }

        private void removeUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectedUser = usersListView.SelectedItems[0].Tag as User;
            users.Remove(selectedUser);
            RefreshUsersTable();
            ConfigChanged(this, new EventArgs());
        }

        private void addGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var addGroupForm = new AddGroupForm(users) { Owner = this.FindForm()})
            {
                if(addGroupForm.ShowDialog() == DialogResult.OK)
                {
                    var newGrp = addGroupForm.GetGroup();

                    if(newGrp != null)
                    {
                        users.Add(newGrp);
                        RefreshUsersTable();
                        ConfigChanged(this, new EventArgs());
                    }
                }
            }
        }
    }
}
