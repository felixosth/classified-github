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

namespace InSupport.Client
{
    public partial class UserInputTextForm : Form
    {
        public string ReturnedText { get; set; }

        public UserInputTextForm(string preTypedText = "")
        {
            InitializeComponent();
            ClientControl.Instance.RegisterUIControlForAutoTheming(this);
            textBox1.Text = preTypedText;
            //label1.Text = labelString;
            //this.Name = title;
        }

        private void UserInputTextForm_Load(object sender, EventArgs e)
        {
            textBox1.Select();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ReturnedText = textBox1.Text;

            DialogResult = DialogResult.OK;
        }
    }
}
