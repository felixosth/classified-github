namespace LSIRaidMonitor
{
    partial class LSIRaidMonitorInstallControl
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.storCliPathTxtBox = new System.Windows.Forms.TextBox();
            this.browseBtn = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.browseBtn);
            this.groupBox1.Controls.Add(this.storCliPathTxtBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(404, 69);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "LSI Raid Config";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "StorCLI.exe path";
            // 
            // storCliPathTxtBox
            // 
            this.storCliPathTxtBox.Location = new System.Drawing.Point(6, 32);
            this.storCliPathTxtBox.Name = "storCliPathTxtBox";
            this.storCliPathTxtBox.ReadOnly = true;
            this.storCliPathTxtBox.Size = new System.Drawing.Size(342, 20);
            this.storCliPathTxtBox.TabIndex = 1;
            // 
            // browseBtn
            // 
            this.browseBtn.Location = new System.Drawing.Point(354, 31);
            this.browseBtn.Name = "browseBtn";
            this.browseBtn.Size = new System.Drawing.Size(44, 21);
            this.browseBtn.TabIndex = 2;
            this.browseBtn.Text = "...";
            this.browseBtn.UseVisualStyleBackColor = true;
            this.browseBtn.Click += new System.EventHandler(this.browseBtn_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "StorCLI, StorCLI64|StorCLI.exe;StorCLI64.exe";
            this.openFileDialog1.InitialDirectory = "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}";
            // 
            // LSIRaidMonitorInstallControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "LSIRaidMonitorInstallControl";
            this.Size = new System.Drawing.Size(423, 98);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button browseBtn;
        private System.Windows.Forms.TextBox storCliPathTxtBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}
