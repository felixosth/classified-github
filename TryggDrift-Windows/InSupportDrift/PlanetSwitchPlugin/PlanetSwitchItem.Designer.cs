namespace PlanetSwitchPlugin
{
    partial class PlanetSwitchItem
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
            this.ipTxtBox = new System.Windows.Forms.TextBox();
            this.passwordTxtBox = new System.Windows.Forms.TextBox();
            this.usernameTxtBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.closeBtn = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ipTxtBox
            // 
            this.ipTxtBox.Location = new System.Drawing.Point(3, 19);
            this.ipTxtBox.Name = "ipTxtBox";
            this.ipTxtBox.Size = new System.Drawing.Size(228, 20);
            this.ipTxtBox.TabIndex = 1;
            // 
            // passwordTxtBox
            // 
            this.passwordTxtBox.Location = new System.Drawing.Point(3, 97);
            this.passwordTxtBox.Name = "passwordTxtBox";
            this.passwordTxtBox.PasswordChar = '•';
            this.passwordTxtBox.Size = new System.Drawing.Size(228, 20);
            this.passwordTxtBox.TabIndex = 3;
            // 
            // usernameTxtBox
            // 
            this.usernameTxtBox.Location = new System.Drawing.Point(3, 58);
            this.usernameTxtBox.Name = "usernameTxtBox";
            this.usernameTxtBox.Size = new System.Drawing.Size(228, 20);
            this.usernameTxtBox.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Password";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Username";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "IP";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.closeBtn);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.ipTxtBox);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.usernameTxtBox);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.passwordTxtBox);
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(286, 130);
            this.panel1.TabIndex = 6;
            // 
            // closeBtn
            // 
            this.closeBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.closeBtn.Location = new System.Drawing.Point(259, 3);
            this.closeBtn.Name = "closeBtn";
            this.closeBtn.Size = new System.Drawing.Size(24, 24);
            this.closeBtn.TabIndex = 6;
            this.closeBtn.Text = "X";
            this.closeBtn.UseVisualStyleBackColor = true;
            this.closeBtn.Click += new System.EventHandler(this.CloseBtn_Click);
            // 
            // PlanetSwitchItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.Controls.Add(this.panel1);
            this.Name = "PlanetSwitchItem";
            this.Size = new System.Drawing.Size(292, 136);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox ipTxtBox;
        private System.Windows.Forms.TextBox passwordTxtBox;
        private System.Windows.Forms.TextBox usernameTxtBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button closeBtn;
    }
}
