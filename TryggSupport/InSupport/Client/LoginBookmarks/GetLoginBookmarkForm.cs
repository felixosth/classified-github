using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VideoOS.Platform;

namespace InSupport.Client.LoginBookmarks
{
    public partial class GetLoginBookmarkForm : Form
    {
        public const string _key = "kdas312o5dk";

        public Login SelectedLogin;

        List<Login> bookmarks;

        public GetLoginBookmarkForm()
        {
            InitializeComponent();
            ClientControl.Instance.RegisterUIControlForAutoTheming(this);

        }

        private void GetLoginBookmarkForm_Load(object sender, EventArgs e)
        {
            LoginSettingsManager.Initialize();


            RefreshLogins();
        }

        private void RefreshLogins()
        {
            listBox1.Items.Clear();

            //if (Properties.Settings.Default.LoginBookmarks == null)
            //{
            //    Properties.Settings.Default.LoginBookmarks = new List<Login>();
            //    Properties.Settings.Default.Save();
            //}

            bookmarks = LoginSettingsManager.GetLogins();
            foreach (var login in bookmarks)
            {
                listBox1.Items.Add(login.SettingsName);
            }
            if(bookmarks.Count > 0)
                listBox1.SelectedIndex = 0;
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (listBox1.SelectedItem == null)
                deleteToolStripMenuItem.Enabled = false;
            else
                deleteToolStripMenuItem.Enabled = true;
        }

        private void addNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var newLogin = new NewLoginBookmark())
            {
                if(newLogin.ShowDialog() == DialogResult.OK)
                {
                    LoginSettingsManager.AddNewlogin(new Login(newLogin.BookmarkName, newLogin.Username, StringCipher.Encrypt(newLogin.Password, _key)));

                    RefreshLogins();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                SelectedLogin = bookmarks[listBox1.SelectedIndex];
            }
            catch(Exception ex)
            {
                if (Properties.Settings.Default.Debug)
                    throw ex;

                SelectedLogin = null;
            }
            this.DialogResult = DialogResult.OK;
        }

        private void listBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(listBox1, e.Location);
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoginSettingsManager.RemoveIndex(listBox1.SelectedIndex);
            RefreshLogins();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }

    [SettingsSerializeAs(SettingsSerializeAs.Xml)]
    public class Login
    {
        public Login(string settingsName, string username, string encryptedPass)
        {
            this.SettingsName = settingsName;
            this.Username = username;
            this.EncryptedPassword = encryptedPass;
        }
        public string SettingsName { get; set; }
        public string Username { get; set; }
        public string EncryptedPassword { get; set; }

        public string DecryptPassword(string passPhrase)
        {
            return StringCipher.Decrypt(EncryptedPassword, passPhrase);
        }
    }
}
