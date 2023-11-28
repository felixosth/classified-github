namespace GenerateOfflineLicense
{
    partial class Form1
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
            this.mguidTxtBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.createLicenseBtn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.licenseTxtBox = new System.Windows.Forms.TextBox();
            this.productTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // mguidTxtBox
            // 
            this.mguidTxtBox.Location = new System.Drawing.Point(12, 25);
            this.mguidTxtBox.Name = "mguidTxtBox";
            this.mguidTxtBox.Size = new System.Drawing.Size(220, 20);
            this.mguidTxtBox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Machine GUID";
            // 
            // createLicenseBtn
            // 
            this.createLicenseBtn.Location = new System.Drawing.Point(157, 101);
            this.createLicenseBtn.Name = "createLicenseBtn";
            this.createLicenseBtn.Size = new System.Drawing.Size(75, 23);
            this.createLicenseBtn.TabIndex = 2;
            this.createLicenseBtn.Text = "Create";
            this.createLicenseBtn.UseVisualStyleBackColor = true;
            this.createLicenseBtn.Click += new System.EventHandler(this.createLicenseBtn_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "License";
            // 
            // licenseTxtBox
            // 
            this.licenseTxtBox.Location = new System.Drawing.Point(12, 64);
            this.licenseTxtBox.Name = "licenseTxtBox";
            this.licenseTxtBox.Size = new System.Drawing.Size(220, 20);
            this.licenseTxtBox.TabIndex = 3;
            // 
            // productTextBox
            // 
            this.productTextBox.Location = new System.Drawing.Point(12, 103);
            this.productTextBox.Name = "productTextBox";
            this.productTextBox.Size = new System.Drawing.Size(139, 20);
            this.productTextBox.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 87);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Product";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(245, 130);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.productTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.licenseTxtBox);
            this.Controls.Add(this.createLicenseBtn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mguidTxtBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Offline License Generator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox mguidTxtBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button createLicenseBtn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox licenseTxtBox;
        private System.Windows.Forms.TextBox productTextBox;
        private System.Windows.Forms.Label label3;
    }
}

