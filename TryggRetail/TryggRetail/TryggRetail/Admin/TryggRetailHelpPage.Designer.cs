namespace TryggRetail.Admin
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
            this.curClientsListView = new System.Windows.Forms.ListView();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.licenseInfoBox = new System.Windows.Forms.TextBox();
            this.licActivationGrp = new System.Windows.Forms.GroupBox();
            this.activateOnlineBtn = new System.Windows.Forms.Button();
            this.licKeyTxtBox = new System.Windows.Forms.TextBox();
            this.licStatusLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.refreshLicenseBtn = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.licActivationGrp.SuspendLayout();
            this.SuspendLayout();
            // 
            // curClientsListView
            // 
            this.curClientsListView.Location = new System.Drawing.Point(6, 19);
            this.curClientsListView.Name = "curClientsListView";
            this.curClientsListView.Size = new System.Drawing.Size(329, 323);
            this.curClientsListView.TabIndex = 1;
            this.curClientsListView.UseCompatibleStateImageBehavior = false;
            this.curClientsListView.View = System.Windows.Forms.View.List;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(260, 348);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Refresh";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.curClientsListView);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Location = new System.Drawing.Point(376, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(341, 377);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Clients using this plugin";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.refreshLicenseBtn);
            this.groupBox2.Controls.Add(this.licenseInfoBox);
            this.groupBox2.Controls.Add(this.licActivationGrp);
            this.groupBox2.Controls.Add(this.licStatusLabel);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(367, 377);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "License Management";
            // 
            // licenseInfoBox
            // 
            this.licenseInfoBox.Location = new System.Drawing.Point(6, 32);
            this.licenseInfoBox.Multiline = true;
            this.licenseInfoBox.Name = "licenseInfoBox";
            this.licenseInfoBox.ReadOnly = true;
            this.licenseInfoBox.Size = new System.Drawing.Size(355, 228);
            this.licenseInfoBox.TabIndex = 4;
            // 
            // licActivationGrp
            // 
            this.licActivationGrp.Controls.Add(this.activateOnlineBtn);
            this.licActivationGrp.Controls.Add(this.licKeyTxtBox);
            this.licActivationGrp.Location = new System.Drawing.Point(6, 295);
            this.licActivationGrp.Name = "licActivationGrp";
            this.licActivationGrp.Size = new System.Drawing.Size(355, 76);
            this.licActivationGrp.TabIndex = 3;
            this.licActivationGrp.TabStop = false;
            this.licActivationGrp.Text = "License activation";
            // 
            // activateOnlineBtn
            // 
            this.activateOnlineBtn.Location = new System.Drawing.Point(241, 45);
            this.activateOnlineBtn.Name = "activateOnlineBtn";
            this.activateOnlineBtn.Size = new System.Drawing.Size(105, 23);
            this.activateOnlineBtn.TabIndex = 3;
            this.activateOnlineBtn.Text = "Activate Online";
            this.activateOnlineBtn.UseVisualStyleBackColor = true;
            this.activateOnlineBtn.Click += new System.EventHandler(this.activateOnlineBtn_Click);
            // 
            // licKeyTxtBox
            // 
            this.licKeyTxtBox.Location = new System.Drawing.Point(6, 19);
            this.licKeyTxtBox.Name = "licKeyTxtBox";
            this.licKeyTxtBox.Size = new System.Drawing.Size(340, 20);
            this.licKeyTxtBox.TabIndex = 2;
            // 
            // licStatusLabel
            // 
            this.licStatusLabel.AutoSize = true;
            this.licStatusLabel.Location = new System.Drawing.Point(52, 16);
            this.licStatusLabel.Name = "licStatusLabel";
            this.licStatusLabel.Size = new System.Drawing.Size(33, 13);
            this.licStatusLabel.TabIndex = 1;
            this.licStatusLabel.Text = "None";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Status:";
            // 
            // refreshLicenseBtn
            // 
            this.refreshLicenseBtn.Location = new System.Drawing.Point(256, 266);
            this.refreshLicenseBtn.Name = "refreshLicenseBtn";
            this.refreshLicenseBtn.Size = new System.Drawing.Size(105, 23);
            this.refreshLicenseBtn.TabIndex = 4;
            this.refreshLicenseBtn.Text = "Refresh license";
            this.refreshLicenseBtn.UseVisualStyleBackColor = true;
            this.refreshLicenseBtn.Click += new System.EventHandler(this.refreshLicenseBtn_Click);
            // 
            // HelpPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "HelpPage";
            this.Size = new System.Drawing.Size(1057, 690);
            this.Load += new System.EventHandler(this.HelpPage_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.licActivationGrp.ResumeLayout(false);
            this.licActivationGrp.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ListView curClientsListView;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox licActivationGrp;
        private System.Windows.Forms.Button activateOnlineBtn;
        private System.Windows.Forms.TextBox licKeyTxtBox;
        private System.Windows.Forms.Label licStatusLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox licenseInfoBox;
        private System.Windows.Forms.Button refreshLicenseBtn;
    }
}
