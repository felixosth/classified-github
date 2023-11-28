using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Windows.Forms;
using VideoOS.Platform;

namespace InSupport.Client
{
    public partial class ProblemBrowser : Form
    {
        List<string> solutions = new List<string>();

        public ProblemBrowser()
        {
            InitializeComponent();

            ClientControl.Instance.RegisterUIControlForAutoTheming(this);
        }

        private void ProblemBrowser_Load(object sender, EventArgs e)
        {
            this.Text = "InSupport Problemlösare";

            var splitter = Resources.Resource1.VanligaFel.Split('|');

            for (int i = 0; i < splitter.Length; i++)
            {
                listBox1.Items.Add(splitter[i]);
                i++;
                solutions.Add(splitter[i].Remove(0,1));
            }
            listBox1.SelectedIndex = 0;

            
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            webBrowser1.DocumentText =
                "<!DOCTYPE html>" +
                "<head>" +
                "<style>" +
                "p { font-family:arial; }" +
                "</style>" +
                "</head><body>" +
                solutions[listBox1.SelectedIndex];
            //webBrowser1.DocumentText += solutions[listBox1.SelectedIndex];
        }
    }
}
