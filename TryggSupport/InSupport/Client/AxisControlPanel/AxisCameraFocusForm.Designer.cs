namespace InSupport.Client
{
    partial class AxisCameraFocusForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AxisCameraFocusForm));
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.usernameBox = new System.Windows.Forms.TextBox();
            this.passwordBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.bookmarksButton = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(191, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Detta fungerar endast på Axis kameror.";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(122, 124);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(90, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Autofokusera";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.bookmarksButton);
            this.groupBox2.Controls.Add(this.usernameBox);
            this.groupBox2.Controls.Add(this.passwordBox);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(12, 25);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 93);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Camera Login";
            // 
            // usernameBox
            // 
            this.usernameBox.Location = new System.Drawing.Point(76, 13);
            this.usernameBox.Name = "usernameBox";
            this.usernameBox.Size = new System.Drawing.Size(118, 20);
            this.usernameBox.TabIndex = 0;
            // 
            // passwordBox
            // 
            this.passwordBox.Location = new System.Drawing.Point(76, 39);
            this.passwordBox.Name = "passwordBox";
            this.passwordBox.PasswordChar = '*';
            this.passwordBox.Size = new System.Drawing.Size(118, 20);
            this.passwordBox.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(6, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Username:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Location = new System.Drawing.Point(6, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Password:";
            // 
            // bookmarksButton
            // 
            this.bookmarksButton.Location = new System.Drawing.Point(157, 65);
            this.bookmarksButton.Name = "bookmarksButton";
            this.bookmarksButton.Size = new System.Drawing.Size(34, 19);
            this.bookmarksButton.TabIndex = 5;
            this.bookmarksButton.Text = "...";
            this.bookmarksButton.UseVisualStyleBackColor = true;
            this.bookmarksButton.Click += new System.EventHandler(this.bookmarksButton_Click);
            // 
            // AxisCameraFocusForm
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(224, 156);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AxisCameraFocusForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Autofokus";
            this.Load += new System.EventHandler(this.AxisCameraFocusForm_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox usernameBox;
        private System.Windows.Forms.TextBox passwordBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button bookmarksButton;
    }
}