namespace InSupport.Drift.Plugins
{
    partial class MilestoneRecLogConfigUserControl
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
            this.label1 = new System.Windows.Forms.Label();
            this.deviceHandlingLogTxtBox = new System.Windows.Forms.TextBox();
            this.browseDeviceHandlingBtn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.storagesTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "DeviceHandling.log";
            // 
            // deviceHandlingLogTxtBox
            // 
            this.deviceHandlingLogTxtBox.Location = new System.Drawing.Point(15, 25);
            this.deviceHandlingLogTxtBox.Name = "deviceHandlingLogTxtBox";
            this.deviceHandlingLogTxtBox.ReadOnly = true;
            this.deviceHandlingLogTxtBox.Size = new System.Drawing.Size(414, 20);
            this.deviceHandlingLogTxtBox.TabIndex = 1;
            this.deviceHandlingLogTxtBox.Text = "C:\\ProgramData\\Milestone\\XProtect Recording Server\\Logs\\DeviceHandling.log";
            // 
            // browseDeviceHandlingBtn
            // 
            this.browseDeviceHandlingBtn.Location = new System.Drawing.Point(435, 23);
            this.browseDeviceHandlingBtn.Name = "browseDeviceHandlingBtn";
            this.browseDeviceHandlingBtn.Size = new System.Drawing.Size(75, 23);
            this.browseDeviceHandlingBtn.TabIndex = 2;
            this.browseDeviceHandlingBtn.Text = "Browse";
            this.browseDeviceHandlingBtn.UseVisualStyleBackColor = true;
            this.browseDeviceHandlingBtn.Click += new System.EventHandler(this.browseDeviceHandlingBtn_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(315, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Local storages (one per line). Do NOT include 2nd level archives.";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 396);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(346, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Example: E:\\MediaDatabase\\6709726d-619f-4ba3-b537-6c5407f8d489";
            // 
            // storagesTextBox
            // 
            this.storagesTextBox.Location = new System.Drawing.Point(15, 64);
            this.storagesTextBox.Multiline = true;
            this.storagesTextBox.Name = "storagesTextBox";
            this.storagesTextBox.Size = new System.Drawing.Size(414, 329);
            this.storagesTextBox.TabIndex = 6;
            // 
            // MilestoneRecLogConfigUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.storagesTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.browseDeviceHandlingBtn);
            this.Controls.Add(this.deviceHandlingLogTxtBox);
            this.Controls.Add(this.label1);
            this.Name = "MilestoneRecLogConfigUserControl";
            this.Size = new System.Drawing.Size(606, 525);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox deviceHandlingLogTxtBox;
        private System.Windows.Forms.Button browseDeviceHandlingBtn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox storagesTextBox;
    }
}
