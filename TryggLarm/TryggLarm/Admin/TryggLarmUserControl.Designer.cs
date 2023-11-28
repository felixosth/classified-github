namespace TryggLarm.Admin
{
    partial class TryggLarmUserControl
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
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.nameBox = new System.Windows.Forms.TextBox();
            this.telBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.emailBox = new System.Windows.Forms.TextBox();
            this.addAlarmBtn = new System.Windows.Forms.Button();
            this.newAlarmBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.sendToEmailChkBox = new System.Windows.Forms.CheckBox();
            this.sendToTelChkBox = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.alarmListView = new System.Windows.Forms.ListView();
            this.alarmListViewContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.emailBodyBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.emailSubjectBox = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.smsMessageBox = new System.Windows.Forms.TextBox();
            this.alarmOffsetNum = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.attachImageChkBox = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.alarmListViewContextMenu.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.alarmOffsetNum)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name:";
            // 
            // nameBox
            // 
            this.nameBox.Location = new System.Drawing.Point(77, 16);
            this.nameBox.Name = "nameBox";
            this.nameBox.Size = new System.Drawing.Size(206, 20);
            this.nameBox.TabIndex = 1;
            this.nameBox.TextChanged += new System.EventHandler(this.OnUserChange);
            // 
            // telBox
            // 
            this.telBox.Location = new System.Drawing.Point(77, 42);
            this.telBox.Name = "telBox";
            this.telBox.Size = new System.Drawing.Size(183, 20);
            this.telBox.TabIndex = 3;
            this.telBox.TextChanged += new System.EventHandler(this.OnUserChange);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Tel(SMS):";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Email:";
            // 
            // emailBox
            // 
            this.emailBox.Location = new System.Drawing.Point(77, 68);
            this.emailBox.Name = "emailBox";
            this.emailBox.Size = new System.Drawing.Size(183, 20);
            this.emailBox.TabIndex = 5;
            this.emailBox.TextChanged += new System.EventHandler(this.OnUserChange);
            // 
            // addAlarmBtn
            // 
            this.addAlarmBtn.Location = new System.Drawing.Point(196, 17);
            this.addAlarmBtn.Name = "addAlarmBtn";
            this.addAlarmBtn.Size = new System.Drawing.Size(87, 23);
            this.addAlarmBtn.TabIndex = 7;
            this.addAlarmBtn.Text = "Add Alarm";
            this.addAlarmBtn.UseVisualStyleBackColor = true;
            this.addAlarmBtn.Click += new System.EventHandler(this.addAlarmBtn_Click);
            // 
            // newAlarmBox
            // 
            this.newAlarmBox.Location = new System.Drawing.Point(6, 19);
            this.newAlarmBox.Name = "newAlarmBox";
            this.newAlarmBox.Size = new System.Drawing.Size(184, 20);
            this.newAlarmBox.TabIndex = 8;
            this.newAlarmBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.newAlarmBox_KeyDown);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.sendToEmailChkBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.sendToTelChkBox);
            this.groupBox1.Controls.Add(this.nameBox);
            this.groupBox1.Controls.Add(this.telBox);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.emailBox);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(289, 102);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Info";
            // 
            // sendToEmailChkBox
            // 
            this.sendToEmailChkBox.AutoSize = true;
            this.sendToEmailChkBox.Location = new System.Drawing.Point(266, 71);
            this.sendToEmailChkBox.Name = "sendToEmailChkBox";
            this.sendToEmailChkBox.Size = new System.Drawing.Size(15, 14);
            this.sendToEmailChkBox.TabIndex = 13;
            this.sendToEmailChkBox.UseVisualStyleBackColor = true;
            this.sendToEmailChkBox.CheckedChanged += new System.EventHandler(this.OnUserChange);
            // 
            // sendToTelChkBox
            // 
            this.sendToTelChkBox.AutoSize = true;
            this.sendToTelChkBox.Location = new System.Drawing.Point(266, 45);
            this.sendToTelChkBox.Name = "sendToTelChkBox";
            this.sendToTelChkBox.Size = new System.Drawing.Size(15, 14);
            this.sendToTelChkBox.TabIndex = 12;
            this.sendToTelChkBox.UseVisualStyleBackColor = true;
            this.sendToTelChkBox.CheckedChanged += new System.EventHandler(this.OnUserChange);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.alarmOffsetNum);
            this.groupBox2.Controls.Add(this.alarmListView);
            this.groupBox2.Controls.Add(this.newAlarmBox);
            this.groupBox2.Controls.Add(this.addAlarmBtn);
            this.groupBox2.Location = new System.Drawing.Point(3, 111);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(289, 297);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Alarm definitions";
            // 
            // alarmListView
            // 
            this.alarmListView.ContextMenuStrip = this.alarmListViewContextMenu;
            this.alarmListView.Location = new System.Drawing.Point(6, 45);
            this.alarmListView.Name = "alarmListView";
            this.alarmListView.Size = new System.Drawing.Size(184, 213);
            this.alarmListView.TabIndex = 9;
            this.alarmListView.UseCompatibleStateImageBehavior = false;
            this.alarmListView.View = System.Windows.Forms.View.List;
            // 
            // alarmListViewContextMenu
            // 
            this.alarmListViewContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeToolStripMenuItem});
            this.alarmListViewContextMenu.Name = "alarmListViewContextMenu";
            this.alarmListViewContextMenu.Size = new System.Drawing.Size(118, 26);
            this.alarmListViewContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.alarmListContextMenu_Opening);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.attachImageChkBox);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.emailBodyBox);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.emailSubjectBox);
            this.groupBox3.Location = new System.Drawing.Point(298, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(443, 253);
            this.groupBox3.TabIndex = 11;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Mail Formatting";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 58);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(31, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Body";
            // 
            // emailBodyBox
            // 
            this.emailBodyBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.emailBodyBox.Location = new System.Drawing.Point(9, 74);
            this.emailBodyBox.Multiline = true;
            this.emailBodyBox.Name = "emailBodyBox";
            this.emailBodyBox.Size = new System.Drawing.Size(428, 143);
            this.emailBodyBox.TabIndex = 2;
            this.emailBodyBox.Text = "Hej %namn%,\r\n\r\nSorry asså men larmet gick idag.\r\n\r\nMed sorgsna hälsningar,\r\nInSup" +
    "port Nätverksvideo AB";
            this.emailBodyBox.TextChanged += new System.EventHandler(this.OnUserChange);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Subject";
            // 
            // emailSubjectBox
            // 
            this.emailSubjectBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.emailSubjectBox.Location = new System.Drawing.Point(9, 32);
            this.emailSubjectBox.Name = "emailSubjectBox";
            this.emailSubjectBox.Size = new System.Drawing.Size(428, 20);
            this.emailSubjectBox.TabIndex = 0;
            this.emailSubjectBox.Text = "New alarm: %alarm%";
            this.emailSubjectBox.TextChanged += new System.EventHandler(this.OnUserChange);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.smsMessageBox);
            this.groupBox4.Location = new System.Drawing.Point(3, 414);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(207, 130);
            this.groupBox4.TabIndex = 12;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "SMS";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(50, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Message";
            // 
            // smsMessageBox
            // 
            this.smsMessageBox.Location = new System.Drawing.Point(6, 32);
            this.smsMessageBox.Multiline = true;
            this.smsMessageBox.Name = "smsMessageBox";
            this.smsMessageBox.Size = new System.Drawing.Size(195, 92);
            this.smsMessageBox.TabIndex = 0;
            this.smsMessageBox.TextChanged += new System.EventHandler(this.OnUserChange);
            // 
            // alarmOffsetNum
            // 
            this.alarmOffsetNum.DecimalPlaces = 1;
            this.alarmOffsetNum.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.alarmOffsetNum.Location = new System.Drawing.Point(121, 264);
            this.alarmOffsetNum.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.alarmOffsetNum.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            -2147483648});
            this.alarmOffsetNum.Name = "alarmOffsetNum";
            this.alarmOffsetNum.Size = new System.Drawing.Size(69, 20);
            this.alarmOffsetNum.TabIndex = 10;
            this.alarmOffsetNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.alarmOffsetNum.ValueChanged += new System.EventHandler(this.OnUserChange);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 266);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(112, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "Alarm timestamp offset";
            // 
            // attachImageChkBox
            // 
            this.attachImageChkBox.AutoSize = true;
            this.attachImageChkBox.Location = new System.Drawing.Point(9, 223);
            this.attachImageChkBox.Name = "attachImageChkBox";
            this.attachImageChkBox.Size = new System.Drawing.Size(161, 17);
            this.attachImageChkBox.TabIndex = 4;
            this.attachImageChkBox.Text = "Attach related camera image";
            this.attachImageChkBox.UseVisualStyleBackColor = true;
            this.attachImageChkBox.CheckedChanged += new System.EventHandler(this.OnUserChange);
            // 
            // TryggLarmUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "TryggLarmUserControl";
            this.Size = new System.Drawing.Size(1107, 615);
            this.Load += new System.EventHandler(this.TryggLarmUserControl_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.alarmListViewContextMenu.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.alarmOffsetNum)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion


        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox nameBox;
        private System.Windows.Forms.TextBox telBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox emailBox;
        private System.Windows.Forms.Button addAlarmBtn;
        private System.Windows.Forms.TextBox newAlarmBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListView alarmListView;
        private System.Windows.Forms.ContextMenuStrip alarmListViewContextMenu;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.CheckBox sendToEmailChkBox;
        private System.Windows.Forms.CheckBox sendToTelChkBox;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox emailSubjectBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox emailBodyBox;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox smsMessageBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown alarmOffsetNum;
        private System.Windows.Forms.CheckBox attachImageChkBox;
    }
}
