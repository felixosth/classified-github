namespace ESLogin.Admin
{
    partial class AddUserForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.authDataTxtBox = new System.Windows.Forms.TextBox();
            this.authDataLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.authenticatorComboBox = new System.Windows.Forms.ComboBox();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.okBtn = new System.Windows.Forms.Button();
            this.rolesCheckListBox = new System.Windows.Forms.CheckedListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.userComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "User";
            // 
            // authDataTxtBox
            // 
            this.authDataTxtBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.authDataTxtBox.Location = new System.Drawing.Point(12, 104);
            this.authDataTxtBox.Name = "authDataTxtBox";
            this.authDataTxtBox.Size = new System.Drawing.Size(255, 20);
            this.authDataTxtBox.TabIndex = 3;
            this.authDataTxtBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.authDataTxtBox_KeyDown);
            // 
            // authDataLabel
            // 
            this.authDataLabel.AutoSize = true;
            this.authDataLabel.Location = new System.Drawing.Point(12, 88);
            this.authDataLabel.Name = "authDataLabel";
            this.authDataLabel.Size = new System.Drawing.Size(94, 13);
            this.authDataLabel.TabIndex = 2;
            this.authDataLabel.Text = "Authenticator data";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Authenticator";
            // 
            // authenticatorComboBox
            // 
            this.authenticatorComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.authenticatorComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.authenticatorComboBox.FormattingEnabled = true;
            this.authenticatorComboBox.Location = new System.Drawing.Point(12, 64);
            this.authenticatorComboBox.Name = "authenticatorComboBox";
            this.authenticatorComboBox.Size = new System.Drawing.Size(255, 21);
            this.authenticatorComboBox.TabIndex = 5;
            this.authenticatorComboBox.SelectedIndexChanged += new System.EventHandler(this.authenticatorComboBox_SelectedIndexChanged);
            // 
            // cancelBtn
            // 
            this.cancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelBtn.Location = new System.Drawing.Point(192, 441);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(75, 23);
            this.cancelBtn.TabIndex = 6;
            this.cancelBtn.Text = "Cancel";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // okBtn
            // 
            this.okBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okBtn.Location = new System.Drawing.Point(111, 441);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(75, 23);
            this.okBtn.TabIndex = 7;
            this.okBtn.Text = "OK";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
            // 
            // rolesCheckListBox
            // 
            this.rolesCheckListBox.CheckOnClick = true;
            this.rolesCheckListBox.FormattingEnabled = true;
            this.rolesCheckListBox.Location = new System.Drawing.Point(12, 143);
            this.rolesCheckListBox.Name = "rolesCheckListBox";
            this.rolesCheckListBox.Size = new System.Drawing.Size(255, 289);
            this.rolesCheckListBox.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 127);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Roles";
            // 
            // userComboBox
            // 
            this.userComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.userComboBox.FormattingEnabled = true;
            this.userComboBox.Location = new System.Drawing.Point(12, 24);
            this.userComboBox.Name = "userComboBox";
            this.userComboBox.Size = new System.Drawing.Size(255, 21);
            this.userComboBox.TabIndex = 10;
            // 
            // AddUserForm
            // 
            this.AcceptButton = this.okBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelBtn;
            this.ClientSize = new System.Drawing.Size(279, 476);
            this.Controls.Add(this.userComboBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.rolesCheckListBox);
            this.Controls.Add(this.okBtn);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.authenticatorComboBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.authDataTxtBox);
            this.Controls.Add(this.authDataLabel);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "AddUserForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add user";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox authDataTxtBox;
        private System.Windows.Forms.Label authDataLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox authenticatorComboBox;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.CheckedListBox rolesCheckListBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox userComboBox;
    }
}