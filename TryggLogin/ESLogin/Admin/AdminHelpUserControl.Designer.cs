namespace ESLogin.Admin
{
    partial class AdminHelpUserControl
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
            this.aboutTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // aboutTextBox
            // 
            this.aboutTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.aboutTextBox.Location = new System.Drawing.Point(0, 0);
            this.aboutTextBox.Multiline = true;
            this.aboutTextBox.Name = "aboutTextBox";
            this.aboutTextBox.ReadOnly = true;
            this.aboutTextBox.Size = new System.Drawing.Size(617, 451);
            this.aboutTextBox.TabIndex = 0;
            this.aboutTextBox.Text = "TryggLogin plugin made by InSupport Nätverksvideo AB 2020.\r\nDo not distribute.";
            // 
            // AdminHelpUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.aboutTextBox);
            this.Name = "AdminHelpUserControl";
            this.Size = new System.Drawing.Size(617, 451);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox aboutTextBox;
    }
}
