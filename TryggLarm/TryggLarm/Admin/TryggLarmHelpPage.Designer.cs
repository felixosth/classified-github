namespace TryggLarm.Admin
{
    partial class HelpPage
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.socketOptionsBox = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.testEmailBox = new System.Windows.Forms.TextBox();
            this.sendTestEmailBtn = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.passwordBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.emailBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.portBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.hostBox = new System.Windows.Forms.TextBox();
            this.saveBtn = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.sendasBox = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.licenseStatusLabel = new System.Windows.Forms.Label();
            this.licenseInfoTxtBox = new System.Windows.Forms.TextBox();
            this.licActivationGrp = new System.Windows.Forms.GroupBox();
            this.activateLicenseBtn = new System.Windows.Forms.Button();
            this.licenseActTxtBox = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.licActivationGrp.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.socketOptionsBox);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.testEmailBox);
            this.groupBox1.Controls.Add(this.sendTestEmailBtn);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.passwordBox);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.emailBox);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.portBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.hostBox);
            this.groupBox1.Location = new System.Drawing.Point(6, 19);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(228, 220);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "SMTP Settings";
            // 
            // socketOptionsBox
            // 
            this.socketOptionsBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.socketOptionsBox.FormattingEnabled = true;
            this.socketOptionsBox.Location = new System.Drawing.Point(101, 64);
            this.socketOptionsBox.Name = "socketOptionsBox";
            this.socketOptionsBox.Size = new System.Drawing.Size(121, 21);
            this.socketOptionsBox.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 173);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(91, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Send test email to";
            // 
            // testEmailBox
            // 
            this.testEmailBox.Location = new System.Drawing.Point(9, 189);
            this.testEmailBox.Name = "testEmailBox";
            this.testEmailBox.Size = new System.Drawing.Size(132, 20);
            this.testEmailBox.TabIndex = 1;
            // 
            // sendTestEmailBtn
            // 
            this.sendTestEmailBtn.Location = new System.Drawing.Point(147, 187);
            this.sendTestEmailBtn.Name = "sendTestEmailBtn";
            this.sendTestEmailBtn.Size = new System.Drawing.Size(75, 23);
            this.sendTestEmailBtn.TabIndex = 1;
            this.sendTestEmailBtn.Text = "Send";
            this.sendTestEmailBtn.UseVisualStyleBackColor = true;
            this.sendTestEmailBtn.Click += new System.EventHandler(this.sendTestEmailBtn_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 134);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Password";
            // 
            // passwordBox
            // 
            this.passwordBox.Location = new System.Drawing.Point(9, 150);
            this.passwordBox.Name = "passwordBox";
            this.passwordBox.PasswordChar = '*';
            this.passwordBox.Size = new System.Drawing.Size(213, 20);
            this.passwordBox.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 95);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Email";
            // 
            // emailBox
            // 
            this.emailBox.Location = new System.Drawing.Point(9, 111);
            this.emailBox.Name = "emailBox";
            this.emailBox.Size = new System.Drawing.Size(213, 20);
            this.emailBox.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Port";
            // 
            // portBox
            // 
            this.portBox.Location = new System.Drawing.Point(41, 64);
            this.portBox.Name = "portBox";
            this.portBox.Size = new System.Drawing.Size(56, 20);
            this.portBox.TabIndex = 1;
            this.portBox.Text = "25";
            this.portBox.TextChanged += new System.EventHandler(this.portBox_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Hostname";
            // 
            // hostBox
            // 
            this.hostBox.Location = new System.Drawing.Point(9, 38);
            this.hostBox.Name = "hostBox";
            this.hostBox.Size = new System.Drawing.Size(213, 20);
            this.hostBox.TabIndex = 1;
            // 
            // saveBtn
            // 
            this.saveBtn.Location = new System.Drawing.Point(401, 216);
            this.saveBtn.Name = "saveBtn";
            this.saveBtn.Size = new System.Drawing.Size(75, 23);
            this.saveBtn.TabIndex = 1;
            this.saveBtn.Text = "Save";
            this.saveBtn.UseVisualStyleBackColor = true;
            this.saveBtn.Click += new System.EventHandler(this.saveBtn_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.sendasBox);
            this.groupBox2.Location = new System.Drawing.Point(240, 19);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(236, 84);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "SMS";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 22);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(46, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "Send as";
            // 
            // sendasBox
            // 
            this.sendasBox.Location = new System.Drawing.Point(6, 38);
            this.sendasBox.MaxLength = 11;
            this.sendasBox.Name = "sendasBox";
            this.sendasBox.Size = new System.Drawing.Size(224, 20);
            this.sendasBox.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textBox1);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.groupBox1);
            this.groupBox3.Controls.Add(this.saveBtn);
            this.groupBox3.Controls.Add(this.groupBox2);
            this.groupBox3.Location = new System.Drawing.Point(3, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(484, 248);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Settings";
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.White;
            this.textBox1.Location = new System.Drawing.Point(240, 122);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(236, 88);
            this.textBox1.TabIndex = 4;
            this.textBox1.Text = "%alarm% - The name of the alarm definition\r\n";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(240, 106);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(85, 13);
            this.label9.TabIndex = 5;
            this.label9.Text = "Formatting guide";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.licenseStatusLabel);
            this.groupBox4.Controls.Add(this.licenseInfoTxtBox);
            this.groupBox4.Controls.Add(this.licActivationGrp);
            this.groupBox4.Location = new System.Drawing.Point(3, 257);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(484, 315);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "License Management";
            // 
            // licenseStatusLabel
            // 
            this.licenseStatusLabel.AutoSize = true;
            this.licenseStatusLabel.Location = new System.Drawing.Point(6, 16);
            this.licenseStatusLabel.Name = "licenseStatusLabel";
            this.licenseStatusLabel.Size = new System.Drawing.Size(69, 13);
            this.licenseStatusLabel.TabIndex = 3;
            this.licenseStatusLabel.Text = "Status: None";
            // 
            // licenseInfoTxtBox
            // 
            this.licenseInfoTxtBox.Location = new System.Drawing.Point(6, 40);
            this.licenseInfoTxtBox.Multiline = true;
            this.licenseInfoTxtBox.Name = "licenseInfoTxtBox";
            this.licenseInfoTxtBox.ReadOnly = true;
            this.licenseInfoTxtBox.Size = new System.Drawing.Size(470, 186);
            this.licenseInfoTxtBox.TabIndex = 2;
            // 
            // licActivationGrp
            // 
            this.licActivationGrp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.licActivationGrp.Controls.Add(this.activateLicenseBtn);
            this.licActivationGrp.Controls.Add(this.licenseActTxtBox);
            this.licActivationGrp.Location = new System.Drawing.Point(6, 232);
            this.licActivationGrp.Name = "licActivationGrp";
            this.licActivationGrp.Size = new System.Drawing.Size(470, 77);
            this.licActivationGrp.TabIndex = 1;
            this.licActivationGrp.TabStop = false;
            this.licActivationGrp.Text = "Activation";
            // 
            // activateLicenseBtn
            // 
            this.activateLicenseBtn.Location = new System.Drawing.Point(356, 45);
            this.activateLicenseBtn.Name = "activateLicenseBtn";
            this.activateLicenseBtn.Size = new System.Drawing.Size(108, 23);
            this.activateLicenseBtn.TabIndex = 1;
            this.activateLicenseBtn.Text = "Activate Online";
            this.activateLicenseBtn.UseVisualStyleBackColor = true;
            this.activateLicenseBtn.Click += new System.EventHandler(this.activateLicenseBtn_Click);
            // 
            // licenseActTxtBox
            // 
            this.licenseActTxtBox.Location = new System.Drawing.Point(6, 19);
            this.licenseActTxtBox.Name = "licenseActTxtBox";
            this.licenseActTxtBox.Size = new System.Drawing.Size(458, 20);
            this.licenseActTxtBox.TabIndex = 0;
            // 
            // HelpPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Name = "HelpPage";
            this.Size = new System.Drawing.Size(901, 698);
            this.Load += new System.EventHandler(this.HelpPage_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.licActivationGrp.ResumeLayout(false);
            this.licActivationGrp.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox testEmailBox;
        private System.Windows.Forms.Button sendTestEmailBtn;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox passwordBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox emailBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox portBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox hostBox;
        private System.Windows.Forms.Button saveBtn;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox sendasBox;
        private System.Windows.Forms.ComboBox socketOptionsBox;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label licenseStatusLabel;
        private System.Windows.Forms.TextBox licenseInfoTxtBox;
        private System.Windows.Forms.GroupBox licActivationGrp;
        private System.Windows.Forms.Button activateLicenseBtn;
        private System.Windows.Forms.TextBox licenseActTxtBox;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label9;
    }
}
