namespace LicenseTester
{
    partial class LicenseInfoForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.statusLabel = new System.Windows.Forms.Label();
            this.expirationLabel = new System.Windows.Forms.Label();
            this.licenseKeyLabel = new System.Windows.Forms.Label();
            this.customerLabel = new System.Windows.Forms.Label();
            this.siteLabel = new System.Windows.Forms.Label();
            this.okBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusLabel.Location = new System.Drawing.Point(12, 9);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(51, 16);
            this.statusLabel.TabIndex = 0;
            this.statusLabel.Text = "Status: ";
            // 
            // expirationLabel
            // 
            this.expirationLabel.AutoSize = true;
            this.expirationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.expirationLabel.Location = new System.Drawing.Point(12, 41);
            this.expirationLabel.Name = "expirationLabel";
            this.expirationLabel.Size = new System.Drawing.Size(103, 16);
            this.expirationLabel.TabIndex = 1;
            this.expirationLabel.Text = "Expiration date: ";
            // 
            // licenseKeyLabel
            // 
            this.licenseKeyLabel.AutoSize = true;
            this.licenseKeyLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.licenseKeyLabel.Location = new System.Drawing.Point(12, 25);
            this.licenseKeyLabel.Name = "licenseKeyLabel";
            this.licenseKeyLabel.Size = new System.Drawing.Size(61, 16);
            this.licenseKeyLabel.TabIndex = 2;
            this.licenseKeyLabel.Text = "License: ";
            // 
            // customerLabel
            // 
            this.customerLabel.AutoSize = true;
            this.customerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.customerLabel.Location = new System.Drawing.Point(12, 57);
            this.customerLabel.Name = "customerLabel";
            this.customerLabel.Size = new System.Drawing.Size(71, 16);
            this.customerLabel.TabIndex = 3;
            this.customerLabel.Text = "Customer: ";
            // 
            // siteLabel
            // 
            this.siteLabel.AutoSize = true;
            this.siteLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.siteLabel.Location = new System.Drawing.Point(12, 73);
            this.siteLabel.Name = "siteLabel";
            this.siteLabel.Size = new System.Drawing.Size(37, 16);
            this.siteLabel.TabIndex = 4;
            this.siteLabel.Text = "Site: ";
            // 
            // okBtn
            // 
            this.okBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okBtn.Location = new System.Drawing.Point(287, 116);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(75, 23);
            this.okBtn.TabIndex = 5;
            this.okBtn.Text = "OK";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
            // 
            // LicenseInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(374, 151);
            this.Controls.Add(this.okBtn);
            this.Controls.Add(this.siteLabel);
            this.Controls.Add(this.customerLabel);
            this.Controls.Add(this.licenseKeyLabel);
            this.Controls.Add(this.expirationLabel);
            this.Controls.Add(this.statusLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "LicenseInfoForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "LicenseInfo";
            this.Load += new System.EventHandler(this.LicenseInfoForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Label expirationLabel;
        private System.Windows.Forms.Label licenseKeyLabel;
        private System.Windows.Forms.Label customerLabel;
        private System.Windows.Forms.Label siteLabel;
        private System.Windows.Forms.Button okBtn;
    }
}