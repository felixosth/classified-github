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
using System.Runtime.Serialization;
using System.Security.Permissions;
using TryggLogin.Admin.SubControls.Dialogs;
using AuthenticationSystem;

namespace TryggLogin.Admin.SubControls
{
    public partial class UsrMgmntUserControl : BaseSubUserControl
    {
        UserManagement userManagement;

        public UsrMgmntUserControl(AdminUserControl parent) : base(parent)
        {
            InitializeComponent();
        }

        override public void Fill(Item item)
        {
            bankIdEnviromentComboBox.SelectedIndexChanged -= OnUserChange;

            userManagement = item.Properties.ContainsKey("users") ? UserManagement.Deserialize(item.Properties["users"]) : new UserManagement();

            bankIdEnviromentComboBox.SelectedItem = item.Properties.ContainsKey("bankidenviroment") ? item.Properties["bankidenviroment"] : "test";

            foreach (var user in userManagement.UserList)
            {
                usersListView.Items.Add(ConvertToListViewItem(user));
            }

            bankIdEnviromentComboBox.SelectedIndexChanged += OnUserChange;
        }

        private ListViewItem ConvertToListViewItem(TryggLoginUser user)
        {
            if (user == null) return new ListViewItem("Null user");

            return new ListViewItem(new[] { user.DisplayName, user.RoleName, user.KeyType.ToString(), user.HashedPassword == "" ? "No" : "Yes" })
            {
                Tag = user
            };
        }

        override public void UpdateItem(Item item)
        {
            var tmpList = new List<TryggLoginUser>();

            foreach(ListViewItem usr in usersListView.Items)
            {
                tmpList.Add(usr.Tag as TryggLoginUser);
            }

            userManagement.UserList = tmpList;

            item.Properties["users"] = userManagement.Serialize();
            item.Properties["bankidenviroment"] = bankIdEnviromentComboBox.SelectedItem as string;
        }

        private void usersListBoxContextMenu_Opening(object sender, CancelEventArgs e)
        {
            deleteToolStripMenuItem.Enabled = usersListView.SelectedItems.Count > 0;
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            usersListView.Items.Remove(usersListView.SelectedItems[0]);
            OnUserChange(this, new EventArgs());
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddNewUserDialog addNewUserDialog = new AddNewUserDialog();
            if(addNewUserDialog.ShowDialog() == DialogResult.OK)
            {
                usersListView.Items.Add(ConvertToListViewItem(addNewUserDialog.ResultUser()));
                OnUserChange(sender, e);
            }
        }

        private void UsersListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if(usersListView.SelectedItems.Count > 0)
            {
                usersListView.Items[usersListView.Items.IndexOf(usersListView.SelectedItems[0])] = ConvertToListViewItem(new AddNewUserDialog().EditUser(usersListView.SelectedItems[0].Tag as TryggLoginUser));
            }
        }
    }

    [Serializable]
    public class UserManagement : ISerializable
    {
        public List<TryggLoginUser> UserList { get; set; }
        private string serializedList { get; set; }

        public UserManagement()
        {
            UserList = new List<TryggLoginUser>();
        }

        public static UserManagement Deserialize(string serialized)
        {
            return MessagingWrapper.Packer.Deserialize<UserManagement>(serialized);
        }
        public string Serialize() => MessagingWrapper.Packer.Serialize(this);

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(UserList), UserList);
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public UserManagement(SerializationInfo info, StreamingContext context)
        {
            UserList = info.GetValue(nameof(UserList), typeof(List<TryggLoginUser>)) as List<TryggLoginUser>;
        }

        public static List<TryggLoginUser> GetUsers()
        {
            var items = Configuration.Instance.GetItemConfigurations(TryggLoginDefinition.TryggLoginPluginId, null, TryggLoginDefinition.TryggLoginKind);
            //var item = Configuration.Instance.GetItem(TryggLoginItemManager.usrObjectId, TryggLoginDefinition.TryggLoginKind);
            if (items == null)
                return null;

            var item = items.Find(x => x.Name == "Users");
            //var userManagement = item.Properties.ContainsKey("users") ? UserManagement.Deserialize(item.Properties["users"]) : null;

            if (item.Properties.ContainsKey("users"))
            {
                return MessagingWrapper.Packer.Deserialize<TryggLogin.Admin.SubControls.UserManagement>(item.Properties["users"]).UserList;
            }

            return null;
        }
    }
    

}
