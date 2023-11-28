namespace TryggDriftStoragePlugin
{
    partial class StoragePluginConfig
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
            this.drivesListBox = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // drivesListBox
            // 
            this.drivesListBox.FormattingEnabled = true;
            this.drivesListBox.Location = new System.Drawing.Point(10, 27);
            this.drivesListBox.Name = "drivesListBox";
            this.drivesListBox.Size = new System.Drawing.Size(155, 139);
            this.drivesListBox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Drives to monitor";
            // 
            // StoragePluginConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.drivesListBox);
            this.Name = "StoragePluginConfig";
            this.Size = new System.Drawing.Size(540, 451);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox drivesListBox;
        private System.Windows.Forms.Label label1;
    }
}
