namespace TryggRetail.Admin
{
    partial class TryggRetailUserControl
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
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.stopSoundOnExitChkBox = new System.Windows.Forms.CheckBox();
            this.loopSoundChkBox = new System.Windows.Forms.CheckBox();
            this.alarmSignalTxtBox = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.playbackOffsetBeforeNum = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.playbackOffsetAfterNum = new System.Windows.Forms.NumericUpDown();
            this.loopDefChkBox = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.windowPropGrp = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.heightNum = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.widthNum = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.posYNum = new System.Windows.Forms.NumericUpDown();
            this.fullscreenChkBox = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.posXNum = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.ackTimeNum = new System.Windows.Forms.NumericUpDown();
            this.alarmNameTxtBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.customActionsBox = new System.Windows.Forms.ListBox();
            this.customActionsContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeActionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addActionBtn = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.buttonBlinkChkBox = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.playbackOffsetBeforeNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.playbackOffsetAfterNum)).BeginInit();
            this.windowPropGrp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.heightNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.widthNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.posYNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.posXNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ackTimeNum)).BeginInit();
            this.customActionsContextMenu.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Item name:";
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(111, 14);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(279, 20);
            this.textBoxName.TabIndex = 1;
            this.textBoxName.TextChanged += new System.EventHandler(this.OnUserChange);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonBlinkChkBox);
            this.groupBox1.Controls.Add(this.groupBox4);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.windowPropGrp);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.ackTimeNum);
            this.groupBox1.Controls.Add(this.alarmNameTxtBox);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(16, 40);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(381, 268);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Alarm";
            // 
            // stopSoundOnExitChkBox
            // 
            this.stopSoundOnExitChkBox.AutoSize = true;
            this.stopSoundOnExitChkBox.Location = new System.Drawing.Point(7, 39);
            this.stopSoundOnExitChkBox.Name = "stopSoundOnExitChkBox";
            this.stopSoundOnExitChkBox.Size = new System.Drawing.Size(136, 17);
            this.stopSoundOnExitChkBox.TabIndex = 24;
            this.stopSoundOnExitChkBox.Text = "Stop sound on exit only";
            this.stopSoundOnExitChkBox.UseVisualStyleBackColor = true;
            this.stopSoundOnExitChkBox.CheckedChanged += new System.EventHandler(this.OnUserChange);
            // 
            // loopSoundChkBox
            // 
            this.loopSoundChkBox.AutoSize = true;
            this.loopSoundChkBox.Location = new System.Drawing.Point(264, 15);
            this.loopSoundChkBox.Name = "loopSoundChkBox";
            this.loopSoundChkBox.Size = new System.Drawing.Size(82, 17);
            this.loopSoundChkBox.TabIndex = 21;
            this.loopSoundChkBox.Text = "Loop sound";
            this.loopSoundChkBox.UseVisualStyleBackColor = true;
            this.loopSoundChkBox.CheckedChanged += new System.EventHandler(this.OnUserChange);
            // 
            // alarmSignalTxtBox
            // 
            this.alarmSignalTxtBox.Location = new System.Drawing.Point(78, 13);
            this.alarmSignalTxtBox.Name = "alarmSignalTxtBox";
            this.alarmSignalTxtBox.Size = new System.Drawing.Size(180, 20);
            this.alarmSignalTxtBox.TabIndex = 20;
            this.alarmSignalTxtBox.TextChanged += new System.EventHandler(this.OnUserChange);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.playbackOffsetBeforeNum);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.playbackOffsetAfterNum);
            this.groupBox2.Controls.Add(this.loopDefChkBox);
            this.groupBox2.Location = new System.Drawing.Point(263, 94);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(111, 99);
            this.groupBox2.TabIndex = 19;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Playback";
            // 
            // playbackOffsetBeforeNum
            // 
            this.playbackOffsetBeforeNum.Location = new System.Drawing.Point(50, 19);
            this.playbackOffsetBeforeNum.Name = "playbackOffsetBeforeNum";
            this.playbackOffsetBeforeNum.Size = new System.Drawing.Size(49, 20);
            this.playbackOffsetBeforeNum.TabIndex = 6;
            this.playbackOffsetBeforeNum.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.playbackOffsetBeforeNum.ValueChanged += new System.EventHandler(this.OnUserChange);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 47);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(32, 13);
            this.label10.TabIndex = 18;
            this.label10.Text = "After:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Before:";
            // 
            // playbackOffsetAfterNum
            // 
            this.playbackOffsetAfterNum.Location = new System.Drawing.Point(50, 45);
            this.playbackOffsetAfterNum.Name = "playbackOffsetAfterNum";
            this.playbackOffsetAfterNum.Size = new System.Drawing.Size(49, 20);
            this.playbackOffsetAfterNum.TabIndex = 17;
            this.playbackOffsetAfterNum.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.playbackOffsetAfterNum.ValueChanged += new System.EventHandler(this.OnUserChange);
            // 
            // loopDefChkBox
            // 
            this.loopDefChkBox.AutoSize = true;
            this.loopDefChkBox.Location = new System.Drawing.Point(6, 76);
            this.loopDefChkBox.Name = "loopDefChkBox";
            this.loopDefChkBox.Size = new System.Drawing.Size(99, 17);
            this.loopDefChkBox.TabIndex = 16;
            this.loopDefChkBox.Text = "Loop by default";
            this.loopDefChkBox.UseVisualStyleBackColor = true;
            this.loopDefChkBox.CheckedChanged += new System.EventHandler(this.OnUserChange);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 16);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(66, 13);
            this.label9.TabIndex = 15;
            this.label9.Text = "Alarm signal:";
            // 
            // windowPropGrp
            // 
            this.windowPropGrp.Controls.Add(this.label7);
            this.windowPropGrp.Controls.Add(this.heightNum);
            this.windowPropGrp.Controls.Add(this.label8);
            this.windowPropGrp.Controls.Add(this.widthNum);
            this.windowPropGrp.Controls.Add(this.label6);
            this.windowPropGrp.Controls.Add(this.posYNum);
            this.windowPropGrp.Controls.Add(this.fullscreenChkBox);
            this.windowPropGrp.Controls.Add(this.label5);
            this.windowPropGrp.Controls.Add(this.posXNum);
            this.windowPropGrp.Location = new System.Drawing.Point(9, 94);
            this.windowPropGrp.Name = "windowPropGrp";
            this.windowPropGrp.Size = new System.Drawing.Size(248, 99);
            this.windowPropGrp.TabIndex = 13;
            this.windowPropGrp.TabStop = false;
            this.windowPropGrp.Text = "Window properties";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(125, 70);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 13);
            this.label7.TabIndex = 18;
            this.label7.Text = "Height:";
            // 
            // heightNum
            // 
            this.heightNum.Location = new System.Drawing.Point(169, 68);
            this.heightNum.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.heightNum.Name = "heightNum";
            this.heightNum.Size = new System.Drawing.Size(69, 20);
            this.heightNum.TabIndex = 17;
            this.heightNum.Value = new decimal(new int[] {
            720,
            0,
            0,
            0});
            this.heightNum.ValueChanged += new System.EventHandler(this.OnUserChange);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 70);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(38, 13);
            this.label8.TabIndex = 16;
            this.label8.Text = "Width:";
            // 
            // widthNum
            // 
            this.widthNum.Location = new System.Drawing.Point(50, 68);
            this.widthNum.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.widthNum.Name = "widthNum";
            this.widthNum.Size = new System.Drawing.Size(69, 20);
            this.widthNum.TabIndex = 15;
            this.widthNum.Value = new decimal(new int[] {
            1280,
            0,
            0,
            0});
            this.widthNum.ValueChanged += new System.EventHandler(this.OnUserChange);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(125, 44);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(38, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Pos Y:";
            // 
            // posYNum
            // 
            this.posYNum.Location = new System.Drawing.Point(169, 42);
            this.posYNum.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.posYNum.Name = "posYNum";
            this.posYNum.Size = new System.Drawing.Size(69, 20);
            this.posYNum.TabIndex = 13;
            this.posYNum.ValueChanged += new System.EventHandler(this.OnUserChange);
            // 
            // fullscreenChkBox
            // 
            this.fullscreenChkBox.AutoSize = true;
            this.fullscreenChkBox.Location = new System.Drawing.Point(9, 19);
            this.fullscreenChkBox.Name = "fullscreenChkBox";
            this.fullscreenChkBox.Size = new System.Drawing.Size(113, 17);
            this.fullscreenChkBox.TabIndex = 9;
            this.fullscreenChkBox.Text = "Fullscreen window";
            this.fullscreenChkBox.UseVisualStyleBackColor = true;
            this.fullscreenChkBox.CheckedChanged += new System.EventHandler(this.fullscreenChkBox_CheckedChanged);
            this.fullscreenChkBox.CheckStateChanged += new System.EventHandler(this.OnUserChange);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 44);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Pos X:";
            // 
            // posXNum
            // 
            this.posXNum.Location = new System.Drawing.Point(50, 42);
            this.posXNum.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.posXNum.Name = "posXNum";
            this.posXNum.Size = new System.Drawing.Size(69, 20);
            this.posXNum.TabIndex = 10;
            this.posXNum.ValueChanged += new System.EventHandler(this.OnUserChange);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 41);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Min. acktime:";
            // 
            // ackTimeNum
            // 
            this.ackTimeNum.Location = new System.Drawing.Point(144, 39);
            this.ackTimeNum.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.ackTimeNum.Name = "ackTimeNum";
            this.ackTimeNum.Size = new System.Drawing.Size(49, 20);
            this.ackTimeNum.TabIndex = 5;
            this.ackTimeNum.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.ackTimeNum.ValueChanged += new System.EventHandler(this.OnUserChange);
            // 
            // alarmNameTxtBox
            // 
            this.alarmNameTxtBox.Location = new System.Drawing.Point(95, 13);
            this.alarmNameTxtBox.Name = "alarmNameTxtBox";
            this.alarmNameTxtBox.Size = new System.Drawing.Size(279, 20);
            this.alarmNameTxtBox.TabIndex = 4;
            this.alarmNameTxtBox.TextChanged += new System.EventHandler(this.OnUserChange);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Alarm name:";
            // 
            // customActionsBox
            // 
            this.customActionsBox.ContextMenuStrip = this.customActionsContextMenu;
            this.customActionsBox.FormattingEnabled = true;
            this.customActionsBox.Location = new System.Drawing.Point(6, 19);
            this.customActionsBox.Name = "customActionsBox";
            this.customActionsBox.Size = new System.Drawing.Size(142, 160);
            this.customActionsBox.TabIndex = 3;
            this.customActionsBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.customActionsBox_MouseDown);
            // 
            // customActionsContextMenu
            // 
            this.customActionsContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeActionToolStripMenuItem});
            this.customActionsContextMenu.Name = "customActionsContextMenu";
            this.customActionsContextMenu.Size = new System.Drawing.Size(156, 26);
            this.customActionsContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.customActionsContextMenu_Opening);
            // 
            // removeActionToolStripMenuItem
            // 
            this.removeActionToolStripMenuItem.Name = "removeActionToolStripMenuItem";
            this.removeActionToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.removeActionToolStripMenuItem.Text = "Remove Action";
            this.removeActionToolStripMenuItem.Click += new System.EventHandler(this.removeActionToolStripMenuItem_Click);
            // 
            // addActionBtn
            // 
            this.addActionBtn.Location = new System.Drawing.Point(60, 193);
            this.addActionBtn.Name = "addActionBtn";
            this.addActionBtn.Size = new System.Drawing.Size(88, 23);
            this.addActionBtn.TabIndex = 4;
            this.addActionBtn.Text = "Add Action...";
            this.addActionBtn.UseVisualStyleBackColor = true;
            this.addActionBtn.Click += new System.EventHandler(this.addActionBtn_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.customActionsBox);
            this.groupBox3.Controls.Add(this.addActionBtn);
            this.groupBox3.Location = new System.Drawing.Point(403, 40);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(154, 222);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Actions";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.stopSoundOnExitChkBox);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.alarmSignalTxtBox);
            this.groupBox4.Controls.Add(this.loopSoundChkBox);
            this.groupBox4.Location = new System.Drawing.Point(9, 199);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(365, 63);
            this.groupBox4.TabIndex = 6;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Signal";
            // 
            // buttonBlinkChkBox
            // 
            this.buttonBlinkChkBox.AutoSize = true;
            this.buttonBlinkChkBox.Location = new System.Drawing.Point(9, 67);
            this.buttonBlinkChkBox.Name = "buttonBlinkChkBox";
            this.buttonBlinkChkBox.Size = new System.Drawing.Size(103, 17);
            this.buttonBlinkChkBox.TabIndex = 24;
            this.buttonBlinkChkBox.Text = "Blink ack-button";
            this.buttonBlinkChkBox.UseVisualStyleBackColor = true;
            this.buttonBlinkChkBox.CheckedChanged += new System.EventHandler(this.OnUserChange);
            // 
            // TryggRetailUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.textBoxName);
            this.Controls.Add(this.label1);
            this.Name = "TryggRetailUserControl";
            this.Size = new System.Drawing.Size(850, 543);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.playbackOffsetBeforeNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.playbackOffsetAfterNum)).EndInit();
            this.windowPropGrp.ResumeLayout(false);
            this.windowPropGrp.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.heightNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.widthNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.posYNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.posXNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ackTimeNum)).EndInit();
            this.customActionsContextMenu.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion


        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox alarmNameTxtBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown playbackOffsetBeforeNum;
        private System.Windows.Forms.NumericUpDown ackTimeNum;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox fullscreenChkBox;
        private System.Windows.Forms.GroupBox windowPropGrp;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown heightNum;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown widthNum;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown posYNum;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown posXNum;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox loopDefChkBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown playbackOffsetAfterNum;
        private System.Windows.Forms.ListBox customActionsBox;
        private System.Windows.Forms.Button addActionBtn;
        private System.Windows.Forms.ContextMenuStrip customActionsContextMenu;
        private System.Windows.Forms.ToolStripMenuItem removeActionToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox alarmSignalTxtBox;
        private System.Windows.Forms.CheckBox loopSoundChkBox;
        private System.Windows.Forms.CheckBox stopSoundOnExitChkBox;
        private System.Windows.Forms.CheckBox buttonBlinkChkBox;
        private System.Windows.Forms.GroupBox groupBox4;
    }
}
