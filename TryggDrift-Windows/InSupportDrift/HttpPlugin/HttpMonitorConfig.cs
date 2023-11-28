using InSupport.Drift.Plugins;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HttpPlugin
{
    public class HttpMonitorConfig : BasePluginConfig
    {
        private System.Windows.Forms.TextBox websitesTxtBox;
        private System.Windows.Forms.Label displayLabel;

        public HttpMonitorConfig()
        {
            InitializeComponent();
        }

        string[] Websites
        {
            get
            {
                List<string> lines = new List<string>();
                foreach (var line in websitesTxtBox.Lines)
                {
                    if (line.Trim() != "")
                    {
                        Uri uri = null;
                        Uri.TryCreate(line, UriKind.Absolute, out uri);

                        if (uri != null)
                            lines.Add(uri.ToString());
                    }
                }
                return lines.ToArray();
            }
        }

        string[] Lines
        {
            get
            {
                List<string> lines = new List<string>();
                foreach (var line in websitesTxtBox.Lines)
                {
                    if (line.Trim() != "")
                        lines.Add(line);
                }
                return lines.ToArray();
            }
        }

        public override string[] GetSettings()
        {
            return new string[]
            {
                HttpMonitor._settingsKey + "=" + JsonConvert.SerializeObject(Websites)
            };
        }

        public override bool ValidateForm()
        {
            foreach (var line in Lines)
            {
                if (!Uri.TryCreate(line, UriKind.Absolute, out _))
                {
                    MessageBox.Show("Invalid input. All sites must start with http or https.", "Invalid input");
                    return false;
                }
            }
            return true;
        }

        private void InitializeComponent()
        {
            this.websitesTxtBox = new System.Windows.Forms.TextBox();
            this.displayLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // websitesTxtBox
            // 
            this.websitesTxtBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.websitesTxtBox.Location = new System.Drawing.Point(3, 29);
            this.websitesTxtBox.Multiline = true;
            this.websitesTxtBox.Name = "websitesTxtBox";
            this.websitesTxtBox.Size = new System.Drawing.Size(262, 189);
            this.websitesTxtBox.TabIndex = 0;
            // 
            // displayLabel
            // 
            this.displayLabel.AutoSize = true;
            this.displayLabel.Location = new System.Drawing.Point(3, 13);
            this.displayLabel.Name = "displayLabel";
            this.displayLabel.Size = new System.Drawing.Size(213, 13);
            this.displayLabel.TabIndex = 1;
            this.displayLabel.Text = "Sites to GET (Start with \'http://\' or \'https://\')";
            // 
            // HttpMonitorConfig
            // 
            this.Controls.Add(this.displayLabel);
            this.Controls.Add(this.websitesTxtBox);
            this.Name = "HttpMonitorConfig";
            this.Size = new System.Drawing.Size(268, 221);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
