namespace TryggLogin.Admin.SubControls
{
    partial class LicenseUserControl
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
            this.label2 = new System.Windows.Forms.Label();
            this.licenseGroup = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.expirationLabel = new System.Windows.Forms.Label();
            this.licenseStatusLabel = new System.Windows.Forms.Label();
            this.activateLicenseBtn = new System.Windows.Forms.Button();
            this.licKeyBox = new System.Windows.Forms.TextBox();
            this.licenseGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(218, 25);
            this.label2.TabIndex = 1;
            this.label2.Text = "License Management";
            // 
            // licenseGroup
            // 
            this.licenseGroup.Controls.Add(this.label5);
            this.licenseGroup.Controls.Add(this.label6);
            this.licenseGroup.Controls.Add(this.expirationLabel);
            this.licenseGroup.Controls.Add(this.licenseStatusLabel);
            this.licenseGroup.Controls.Add(this.activateLicenseBtn);
            this.licenseGroup.Controls.Add(this.licKeyBox);
            this.licenseGroup.Location = new System.Drawing.Point(8, 28);
            this.licenseGroup.Name = "licenseGroup";
            this.licenseGroup.Size = new System.Drawing.Size(395, 158);
            this.licenseGroup.TabIndex = 16;
            this.licenseGroup.TabStop = false;
            this.licenseGroup.Text = "License";
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
            // expirationLabel
            // 
            this.expirationLabel.AutoSize = true;
            this.expirationLabel.Location = new System.Drawing.Point(104, 55);
            this.expirationLabel.Name = "expirationLabel";
            this.expirationLabel.Size = new System.Drawing.Size(61, 13);
            this.expirationLabel.TabIndex = 3;
            this.expirationLabel.Text = "1900-01-01";
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
            // activateLicenseBtn
            // 
            this.activateLicenseBtn.Location = new System.Drawing.Point(281, 106);
            this.activateLicenseBtn.Name = "activateLicenseBtn";
            this.activateLicenseBtn.Size = new System.Drawing.Size(90, 23);
            this.activateLicenseBtn.TabIndex = 11;
            this.activateLicenseBtn.Text = "Activate";
            this.activateLicenseBtn.UseVisualStyleBackColor = true;
            this.activateLicenseBtn.Click += new System.EventHandler(this.ActivateLicenseBtn_Click);
            // 
            // licKeyBox
            // 
            this.licKeyBox.Location = new System.Drawing.Point(21, 80);
            this.licKeyBox.Name = "licKeyBox";
            this.licKeyBox.Size = new System.Drawing.Size(350, 20);
            this.licKeyBox.TabIndex = 10;
            // 
            // LicenseUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.licenseGroup);
            this.Controls.Add(this.label2);
            this.Name = "LicenseUserControl";
            this.Size = new System.Drawing.Size(734, 571);
            this.Load += new System.EventHandler(this.LicenseUserControl_Load);
            this.licenseGroup.ResumeLayout(false);
            this.licenseGroup.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox licenseGroup;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label expirationLabel;
        private System.Windows.Forms.Label licenseStatusLabel;
        private System.Windows.Forms.Button activateLicenseBtn;
        private System.Windows.Forms.TextBox licKeyBox;
    }
}
