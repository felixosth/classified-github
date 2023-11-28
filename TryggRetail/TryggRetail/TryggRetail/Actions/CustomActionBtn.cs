using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TryggRetail.Actions
{
    public partial class CustomActionBtn : UserControl
    {
        DateTime lastClick;

        CustomAction myAction;

        public CustomActionBtn(CustomAction action)
        {
            InitializeComponent();
            myAction = action;
            actionButton.Text = action.ToString();

            actionButton.ForeColor = myAction.ButtonForeColor;
            actionButton.BackColor = myAction.ButtonBackColor;
        }

        Random random = new Random();
        private void button1_Click(object sender, EventArgs e)
        {
            if (lastClick.AddSeconds(1) > DateTime.Now)  // no spam allowed >:(
                return;
            //actionButton.ForeColor = Color.FromArgb(random.Next(0,255), random.Next(0,255), random.Next(0, 255));
            lastClick = DateTime.Now;
            myAction.Execute();
        }
    }
}
