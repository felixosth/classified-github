namespace InSupport.Drift.Plugins
{
    partial class TryggDriftMilestoneSettings
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
            this.usrBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.passBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.srvBox = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.useWindowsUsrChkBox = new System.Windows.Forms.CheckBox();
            this.sendAlarmsCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // usrBox
            // 
            this.usrBox.Location = new System.Drawing.Point(3, 55);
            this.usrBox.Name = "usrBox";
            this.usrBox.Size = new System.Drawing.Size(159, 20);
            this.usrBox.TabIndex = 2;
            this.usrBox.TextChanged += new System.EventHandler(this.ChangeTestResult);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Milestone Username";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Milestone Password";
            // 
            // passBox
            // 
            this.passBox.Location = new System.Drawing.Point(3, 94);
            this.passBox.Name = "passBox";
            this.passBox.PasswordChar = '*';
            this.passBox.Size = new System.Drawing.Size(159, 20);
            this.passBox.TabIndex = 3;
            this.passBox.TextChanged += new System.EventHandler(this.ChangeTestResult);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Milestone Server";
            // 
            // srvBox
            // 
            this.srvBox.Location = new System.Drawing.Point(3, 16);
            this.srvBox.Name = "srvBox";
            this.srvBox.Size = new System.Drawing.Size(159, 20);
            this.srvBox.TabIndex = 1;
            this.srvBox.TextChanged += new System.EventHandler(this.ChangeTestResult);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(3, 143);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(159, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "Test connection";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // useWindowsUsrChkBox
            // 
            this.useWindowsUsrChkBox.AutoSize = true;
            this.useWindowsUsrChkBox.Location = new System.Drawing.Point(6, 120);
            this.useWindowsUsrChkBox.Name = "useWindowsUsrChkBox";
            this.useWindowsUsrChkBox.Size = new System.Drawing.Size(117, 17);
            this.useWindowsUsrChkBox.TabIndex = 4;
            this.useWindowsUsrChkBox.Text = "Use Windows login";
            this.useWindowsUsrChkBox.UseVisualStyleBackColor = true;
            this.useWindowsUsrChkBox.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // sendAlarmsCheckBox
            // 
            this.sendAlarmsCheckBox.AutoSize = true;
            this.sendAlarmsCheckBox.Location = new System.Drawing.Point(3, 172);
            this.sendAlarmsCheckBox.Name = "sendAlarmsCheckBox";
            this.sendAlarmsCheckBox.Size = new System.Drawing.Size(145, 17);
            this.sendAlarmsCheckBox.TabIndex = 8;
            this.sendAlarmsCheckBox.Text = "Send alarms to TryggDrift";
            this.sendAlarmsCheckBox.UseVisualStyleBackColor = true;
            // 
            // TryggDriftMilestoneSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.sendAlarmsCheckBox);
            this.Controls.Add(this.useWindowsUsrChkBox);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.srvBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.passBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.usrBox);
            this.Name = "TryggDriftMilestoneSettings";
            this.Size = new System.Drawing.Size(210, 280);
            this.Load += new System.EventHandler(this.TryggDriftMilestoneSettings_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox usrBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox passBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox srvBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox useWindowsUsrChkBox;
        private System.Windows.Forms.CheckBox sendAlarmsCheckBox;
    }
}
