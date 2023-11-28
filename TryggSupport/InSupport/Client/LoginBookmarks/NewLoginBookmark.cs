using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VideoOS.Platform;

namespace InSupport.Client.LoginBookmarks
{
    public partial class NewLoginBookmark : Form
    {
        public string BookmarkName, Username, Password;

        public NewLoginBookmark()
        {
            InitializeComponent();

            ClientControl.Instance.RegisterUIControlForAutoTheming(this);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            BookmarkName = nameBox.Text;
            Username = userBox.Text;
            Password = passBox.Text;

            this.DialogResult = DialogResult.OK;
        }

        private void NewLoginBookmark_Load(object sender, EventArgs e)
        {

        }
    }
}
