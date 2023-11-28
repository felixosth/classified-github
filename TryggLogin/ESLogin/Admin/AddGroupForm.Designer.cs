namespace ESLogin.Admin
{
    partial class AddGroupForm
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
            this.groupsComboBox = new System.Windows.Forms.ComboBox();
            this.rolesCheckListBox = new System.Windows.Forms.CheckedListBox();
            this.okBtn = new System.Windows.Forms.Button();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.authenticatorComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // groupsComboBox
            // 
            this.groupsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.groupsComboBox.FormattingEnabled = true;
            this.groupsComboBox.Location = new System.Drawing.Point(12, 25);
            this.groupsComboBox.Name = "groupsComboBox";
            this.groupsComboBox.Size = new System.Drawing.Size(255, 21);
            this.groupsComboBox.TabIndex = 0;
            // 
            // rolesCheckListBox
            // 
            this.rolesCheckListBox.CheckOnClick = true;
            this.rolesCheckListBox.FormattingEnabled = true;
            this.rolesCheckListBox.Location = new System.Drawing.Point(12, 105);
            this.rolesCheckListBox.Name = "rolesCheckListBox";
            this.rolesCheckListBox.Size = new System.Drawing.Size(255, 319);
            this.rolesCheckListBox.TabIndex = 9;
            // 
            // okBtn
            // 
            this.okBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okBtn.Location = new System.Drawing.Point(111, 441);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(75, 23);
            this.okBtn.TabIndex = 11;
            this.okBtn.Text = "OK";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
            // 
            // cancelBtn
            // 
            this.cancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelBtn.Location = new System.Drawing.Point(192, 441);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(75, 23);
            this.cancelBtn.TabIndex = 10;
            this.cancelBtn.Text = "Cancel";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Group";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 89);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Roles";
            // 
            // authenticatorComboBox
            // 
            this.authenticatorComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.authenticatorComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.authenticatorComboBox.FormattingEnabled = true;
            this.authenticatorComboBox.Location = new System.Drawing.Point(12, 65);
            this.authenticatorComboBox.Name = "authenticatorComboBox";
            this.authenticatorComboBox.Size = new System.Drawing.Size(255, 21);
            this.authenticatorComboBox.TabIndex = 15;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Authenticator";
            // 
            // AddGroupForm
            // 
            this.AcceptButton = this.okBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelBtn;
            this.ClientSize = new System.Drawing.Size(279, 476);
            this.Controls.Add(this.authenticatorComboBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.okBtn);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.rolesCheckListBox);
            this.Controls.Add(this.groupsComboBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "AddGroupForm";
            this.Text = "Add group";
            this.Load += new System.EventHandler(this.AddGroupForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox groupsComboBox;
        private System.Windows.Forms.CheckedListBox rolesCheckListBox;
        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox authenticatorComboBox;
        private System.Windows.Forms.Label label3;
    }
}