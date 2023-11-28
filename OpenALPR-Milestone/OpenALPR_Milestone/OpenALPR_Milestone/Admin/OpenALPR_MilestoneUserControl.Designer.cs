namespace OpenALPR_Milestone.Admin
{
    partial class OpenALPR_MilestoneUserControl
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
            this.companyIdTxtBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.siteTxtBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.setCamerasBtn = new System.Windows.Forms.Button();
            this.lprEventTxtBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.setEventBtn = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.alarmMethodRadio = new System.Windows.Forms.RadioButton();
            this.analyticMethodRadio = new System.Windows.Forms.RadioButton();
            this.licenseGroup = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.licKeyBox = new System.Windows.Forms.TextBox();
            this.activateLicenseBtn = new System.Windows.Forms.Button();
            this.licenseStatusLabel = new System.Windows.Forms.Label();
            this.expirationLabel = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.licenseGroup.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // companyIdTxtBox
            // 
            this.companyIdTxtBox.Location = new System.Drawing.Point(92, 23);
            this.companyIdTxtBox.Name = "companyIdTxtBox";
            this.companyIdTxtBox.Size = new System.Drawing.Size(279, 20);
            this.companyIdTxtBox.TabIndex = 1;
            this.companyIdTxtBox.TextChanged += new System.EventHandler(this.OnUserChange);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Company ID:";
            // 
            // siteTxtBox
            // 
            this.siteTxtBox.Location = new System.Drawing.Point(92, 49);
            this.siteTxtBox.Name = "siteTxtBox";
            this.siteTxtBox.Size = new System.Drawing.Size(279, 20);
            this.siteTxtBox.TabIndex = 2;
            this.siteTxtBox.TextChanged += new System.EventHandler(this.OnUserChange);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Site:";
            // 
            // setCamerasBtn
            // 
            this.setCamerasBtn.Location = new System.Drawing.Point(92, 101);
            this.setCamerasBtn.Name = "setCamerasBtn";
            this.setCamerasBtn.Size = new System.Drawing.Size(89, 23);
            this.setCamerasBtn.TabIndex = 5;
            this.setCamerasBtn.Text = "Set cameras";
            this.setCamerasBtn.UseVisualStyleBackColor = true;
            this.setCamerasBtn.Click += new System.EventHandler(this.setCamerasBtn_Click);
            // 
            // lprEventTxtBox
            // 
            this.lprEventTxtBox.Location = new System.Drawing.Point(92, 75);
            this.lprEventTxtBox.Name = "lprEventTxtBox";
            this.lprEventTxtBox.ReadOnly = true;
            this.lprEventTxtBox.Size = new System.Drawing.Size(244, 20);
            this.lprEventTxtBox.TabIndex = 3;
            this.lprEventTxtBox.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Event";
            // 
            // setEventBtn
            // 
            this.setEventBtn.Location = new System.Drawing.Point(342, 75);
            this.setEventBtn.Name = "setEventBtn";
            this.setEventBtn.Size = new System.Drawing.Size(29, 20);
            this.setEventBtn.TabIndex = 4;
            this.setEventBtn.Text = "...";
            this.setEventBtn.UseVisualStyleBackColor = true;
            this.setEventBtn.Click += new System.EventHandler(this.SetEventBtn_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 140);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Method";
            // 
            // alarmMethodRadio
            // 
            this.alarmMethodRadio.AutoSize = true;
            this.alarmMethodRadio.Checked = true;
            this.alarmMethodRadio.Location = new System.Drawing.Point(92, 138);
            this.alarmMethodRadio.Name = "alarmMethodRadio";
            this.alarmMethodRadio.Size = new System.Drawing.Size(51, 17);
            this.alarmMethodRadio.TabIndex = 6;
            this.alarmMethodRadio.TabStop = true;
            this.alarmMethodRadio.Text = "Alarm";
            this.alarmMethodRadio.UseVisualStyleBackColor = true;
            this.alarmMethodRadio.CheckedChanged += new System.EventHandler(this.OnUserChange);
            // 
            // analyticMethodRadio
            // 
            this.analyticMethodRadio.AutoSize = true;
            this.analyticMethodRadio.Location = new System.Drawing.Point(149, 138);
            this.analyticMethodRadio.Name = "analyticMethodRadio";
            this.analyticMethodRadio.Size = new System.Drawing.Size(90, 17);
            this.analyticMethodRadio.TabIndex = 7;
            this.analyticMethodRadio.Text = "AnalyticEvent";
            this.analyticMethodRadio.UseVisualStyleBackColor = true;
            this.analyticMethodRadio.CheckedChanged += new System.EventHandler(this.OnUserChange);
            // 
            // licenseGroup
            // 
            this.licenseGroup.Controls.Add(this.label5);
            this.licenseGroup.Controls.Add(this.label6);
            this.licenseGroup.Controls.Add(this.expirationLabel);
            this.licenseGroup.Controls.Add(this.licenseStatusLabel);
            this.licenseGroup.Controls.Add(this.activateLicenseBtn);
            this.licenseGroup.Controls.Add(this.licKeyBox);
            this.licenseGroup.Location = new System.Drawing.Point(12, 196);
            this.licenseGroup.Name = "licenseGroup";
            this.licenseGroup.Size = new System.Drawing.Size(395, 158);
            this.licenseGroup.TabIndex = 15;
            this.licenseGroup.TabStop = false;
            this.licenseGroup.Text = "License";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.analyticMethodRadio);
            this.groupBox2.Controls.Add(this.companyIdTxtBox);
            this.groupBox2.Controls.Add(this.alarmMethodRadio);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.siteTxtBox);
            this.groupBox2.Controls.Add(this.setEventBtn);
            this.groupBox2.Controls.Add(this.setCamerasBtn);
            this.groupBox2.Controls.Add(this.lprEventTxtBox);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(12, 13);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(395, 177);
            this.groupBox2.TabIndex = 16;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Configuration";
            // 
            // licKeyBox
            // 
            this.licKeyBox.Location = new System.Drawing.Point(21, 80);
            this.licKeyBox.Name = "licKeyBox";
            this.licKeyBox.Size = new System.Drawing.Size(350, 20);
            this.licKeyBox.TabIndex = 10;
            // 
            // activateLicenseBtn
            // 
            this.activateLicenseBtn.Location = new System.Drawing.Point(281, 106);
            this.activateLicenseBtn.Name = "activateLicenseBtn";
            this.activateLicenseBtn.Size = new System.Drawing.Size(90, 23);
            this.activateLicenseBtn.TabIndex = 11;
            this.activateLicenseBtn.Text = "Activate";
            this.activateLicenseBtn.UseVisualStyleBackColor = true;
            this.activateLicenseBtn.Click += new System.EventHandler(this.Button1_Click);
            // 
            // licenseStatusLabel
            // 
            this.licenseStatusLabel.AutoSize = true;
            this.licenseStatusLabel.Location = new System.Drawing.Point(104, 29);
            this.licenseStatusLabel.Name = "licenseStatusLabel";
            this.licenseStatusLabel.Size = new System.Drawing.Size(33, 13);
            this.licenseStatusLabel.TabIndex = 2;
            this.licenseStatusLabel.Text = "None";
            // 
            // expirationLabel
            // 
            this.expirationLabel.AutoSize = true;
            this.expirationLabel.Location = new System.Drawing.Point(104, 55);
            this.expirationLabel.Name = "expirationLabel";
            this.expirationLabel.Size = new System.Drawing.Size(61, 13);
            this.expirationLabel.TabIndex = 3;
            this.expirationLabel.Text = "1900-01-01";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(18, 55);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Expiration date:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(18, 29);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(78, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "License status:";
            // 
            // OpenALPR_MilestoneUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.licenseGroup);
            this.Name = "OpenALPR_MilestoneUserControl";
            this.Size = new System.Drawing.Size(510, 458);
            this.Load += new System.EventHandler(this.OpenALPR_MilestoneUserControl_Load);
            this.licenseGroup.ResumeLayout(false);
            this.licenseGroup.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TextBox companyIdTxtBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox siteTxtBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button setCamerasBtn;
        private System.Windows.Forms.TextBox lprEventTxtBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button setEventBtn;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton alarmMethodRadio;
        private System.Windows.Forms.RadioButton analyticMethodRadio;
        private System.Windows.Forms.GroupBox licenseGroup;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button activateLicenseBtn;
        private System.Windows.Forms.TextBox licKeyBox;
        private System.Windows.Forms.Label expirationLabel;
        private System.Windows.Forms.Label licenseStatusLabel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
    }
}
