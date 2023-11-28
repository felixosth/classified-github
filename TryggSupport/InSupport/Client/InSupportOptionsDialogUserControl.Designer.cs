namespace InSupport.Client
{
    partial class InSupportOptionsDialogUserControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.ntpServerTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.aboutLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(3, 3);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(64, 20);
            this.checkBox1.TabIndex = 3;
            this.checkBox1.Text = "Debug";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(3, 29);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(149, 20);
            this.checkBox2.TabIndex = 4;
            this.checkBox2.Text = "Enable Control Panel";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // ntpServerTextBox
            // 
            this.ntpServerTextBox.Location = new System.Drawing.Point(3, 71);
            this.ntpServerTextBox.Name = "ntpServerTextBox";
            this.ntpServerTextBox.Size = new System.Drawing.Size(149, 22);
            this.ntpServerTextBox.TabIndex = 5;
            this.ntpServerTextBox.TextChanged += new System.EventHandler(this.ntpServerTextBox_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 16);
            this.label1.TabIndex = 6;
            this.label1.Text = "NTP Server:";
            // 
            // aboutLabel
            // 
            this.aboutLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.aboutLabel.AutoSize = true;
            this.aboutLabel.Location = new System.Drawing.Point(3, 160);
            this.aboutLabel.Name = "aboutLabel";
            this.aboutLabel.Size = new System.Drawing.Size(413, 48);
            this.aboutLabel.TabIndex = 7;
            this.aboutLabel.Text = "Plugin utvecklat av Felix Östh på Insupport Nätverksvideo AB.\r\n\r\nSkicka gärna öns" +
    "kemål och/eller klagomål till felix.osth@insupport.se!";
            // 
            // InSupportOptionsDialogUserControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.aboutLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ntpServerTextBox);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.checkBox1);
            this.Font = new System.Drawing.Font("Arial", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "InSupportOptionsDialogUserControl";
            this.Size = new System.Drawing.Size(432, 217);
            this.Load += new System.EventHandler(this.InSupportOptionsDialogUserControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.TextBox ntpServerTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label aboutLabel;
    }
}
