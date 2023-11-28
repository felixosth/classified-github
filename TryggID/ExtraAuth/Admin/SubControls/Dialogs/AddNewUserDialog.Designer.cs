namespace TryggLogin.Admin.SubControls.Dialogs
{
    partial class AddNewUserDialog
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
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.keyTypeComboBox = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.passwordTxtBox = new System.Windows.Forms.TextBox();
            this.roleComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.idKeyTxtBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.displayNameTxtBox = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.okBtn = new System.Windows.Forms.Button();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.keyTypeComboBox);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.passwordTxtBox);
            this.groupBox1.Controls.Add(this.roleComboBox);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.idKeyTxtBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.displayNameTxtBox);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(230, 190);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "New user";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(128, 55);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Key type";
            this.toolTip1.SetToolTip(this.label5, "BankID Personalnumber or Yubikey OTP key");
            // 
            // keyTypeComboBox
            // 
            this.keyTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.keyTypeComboBox.FormattingEnabled = true;
            this.keyTypeComboBox.Location = new System.Drawing.Point(131, 71);
            this.keyTypeComboBox.Name = "keyTypeComboBox";
            this.keyTypeComboBox.Size = new System.Drawing.Size(93, 21);
            this.keyTypeComboBox.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 94);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(101, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Password (Optional)";
            // 
            // passwordTxtBox
            // 
            this.passwordTxtBox.Location = new System.Drawing.Point(6, 110);
            this.passwordTxtBox.Name = "passwordTxtBox";
            this.passwordTxtBox.Size = new System.Drawing.Size(218, 20);
            this.passwordTxtBox.TabIndex = 2;
            // 
            // roleComboBox
            // 
            this.roleComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.roleComboBox.FormattingEnabled = true;
            this.roleComboBox.Location = new System.Drawing.Point(6, 149);
            this.roleComboBox.Name = "roleComboBox";
            this.roleComboBox.Size = new System.Drawing.Size(218, 21);
            this.roleComboBox.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 133);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Role";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "ID Key (?)";
            this.toolTip1.SetToolTip(this.label2, "BankID Personalnumber or Yubikey OTP key");
            // 
            // idKeyTxtBox
            // 
            this.idKeyTxtBox.Location = new System.Drawing.Point(6, 71);
            this.idKeyTxtBox.Name = "idKeyTxtBox";
            this.idKeyTxtBox.Size = new System.Drawing.Size(119, 20);
            this.idKeyTxtBox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Display Name";
            // 
            // displayNameTxtBox
            // 
            this.displayNameTxtBox.Location = new System.Drawing.Point(6, 32);
            this.displayNameTxtBox.Name = "displayNameTxtBox";
            this.displayNameTxtBox.Size = new System.Drawing.Size(218, 20);
            this.displayNameTxtBox.TabIndex = 0;
            // 
            // okBtn
            // 
            this.okBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okBtn.Location = new System.Drawing.Point(82, 214);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(75, 23);
            this.okBtn.TabIndex = 4;
            this.okBtn.Text = "OK";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.button1_Click);
            // 
            // cancelBtn
            // 
            this.cancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelBtn.Location = new System.Drawing.Point(163, 214);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(75, 23);
            this.cancelBtn.TabIndex = 6;
            this.cancelBtn.Text = "Cancel";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.button2_Click);
            // 
            // AddNewUserDialog
            // 
            this.AcceptButton = this.okBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(250, 249);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.okBtn);
            this.Controls.Add(this.groupBox1);
            this.Name = "AddNewUserDialog";
            this.Text = "AddNewUser";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox roleComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TextBox idKeyTxtBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox displayNameTxtBox;
        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox passwordTxtBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox keyTypeComboBox;
    }
}