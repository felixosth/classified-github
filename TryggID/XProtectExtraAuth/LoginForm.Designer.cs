namespace XProtectExtraAuth
{
    partial class LoginForm
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
            this.loginBtn = new System.Windows.Forms.Button();
            this.serverTxtBox = new System.Windows.Forms.TextBox();
            this.personalNrBox = new System.Windows.Forms.TextBox();
            this.Server = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.infoLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // loginBtn
            // 
            this.loginBtn.Location = new System.Drawing.Point(110, 113);
            this.loginBtn.Name = "loginBtn";
            this.loginBtn.Size = new System.Drawing.Size(88, 23);
            this.loginBtn.TabIndex = 4;
            this.loginBtn.Text = "Initiate Login";
            this.loginBtn.UseVisualStyleBackColor = true;
            this.loginBtn.Click += new System.EventHandler(this.loginBtn_Click);
            // 
            // serverTxtBox
            // 
            this.serverTxtBox.Location = new System.Drawing.Point(12, 25);
            this.serverTxtBox.Name = "serverTxtBox";
            this.serverTxtBox.Size = new System.Drawing.Size(186, 20);
            this.serverTxtBox.TabIndex = 1;
            this.serverTxtBox.Text = "localhost";
            // 
            // personalNrBox
            // 
            this.personalNrBox.Location = new System.Drawing.Point(12, 64);
            this.personalNrBox.Name = "personalNrBox";
            this.personalNrBox.Size = new System.Drawing.Size(186, 20);
            this.personalNrBox.TabIndex = 3;
            // 
            // Server
            // 
            this.Server.AutoSize = true;
            this.Server.Location = new System.Drawing.Point(12, 9);
            this.Server.Name = "Server";
            this.Server.Size = new System.Drawing.Size(38, 13);
            this.Server.TabIndex = 4;
            this.Server.Text = "Server";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Personnummer";
            // 
            // infoLabel
            // 
            this.infoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.infoLabel.AutoSize = true;
            this.infoLabel.Location = new System.Drawing.Point(12, 118);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(0, 13);
            this.infoLabel.TabIndex = 7;
            // 
            // LoginForm
            // 
            this.AcceptButton = this.loginBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(207, 145);
            this.Controls.Add(this.infoLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Server);
            this.Controls.Add(this.personalNrBox);
            this.Controls.Add(this.serverTxtBox);
            this.Controls.Add(this.loginBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "XProtect+BankID";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button loginBtn;
        private System.Windows.Forms.TextBox serverTxtBox;
        private System.Windows.Forms.TextBox personalNrBox;
        private System.Windows.Forms.Label Server;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label infoLabel;
    }
}

