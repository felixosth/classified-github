namespace TryggLarm.NodeEditors
{
    partial class EmailRecipientNodeEditor
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
            this.formattingTxtBox = new System.Windows.Forms.TextBox();
            this.emailTxtBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.nameTxtBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.subjectTxtBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.attachCamImgChkBox = new System.Windows.Forms.CheckBox();
            this.alarmOffsetNum = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.cooldownNum = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.enabledChkBox = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.alarmOffsetNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cooldownNum)).BeginInit();
            this.SuspendLayout();
            // 
            // formattingTxtBox
            // 
            this.formattingTxtBox.Location = new System.Drawing.Point(6, 71);
            this.formattingTxtBox.Multiline = true;
            this.formattingTxtBox.Name = "formattingTxtBox";
            this.formattingTxtBox.Size = new System.Drawing.Size(218, 133);
            this.formattingTxtBox.TabIndex = 13;
            // 
            // emailTxtBox
            // 
            this.emailTxtBox.Location = new System.Drawing.Point(3, 78);
            this.emailTxtBox.Name = "emailTxtBox";
            this.emailTxtBox.Size = new System.Drawing.Size(230, 20);
            this.emailTxtBox.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Email";
            // 
            // nameTxtBox
            // 
            this.nameTxtBox.Location = new System.Drawing.Point(3, 39);
            this.nameTxtBox.Name = "nameTxtBox";
            this.nameTxtBox.Size = new System.Drawing.Size(230, 20);
            this.nameTxtBox.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Name";
            // 
            // subjectTxtBox
            // 
            this.subjectTxtBox.Location = new System.Drawing.Point(6, 32);
            this.subjectTxtBox.Name = "subjectTxtBox";
            this.subjectTxtBox.Size = new System.Drawing.Size(218, 20);
            this.subjectTxtBox.TabIndex = 6;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.subjectTxtBox);
            this.groupBox1.Controls.Add(this.formattingTxtBox);
            this.groupBox1.Location = new System.Drawing.Point(3, 166);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(230, 210);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Formatting";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(31, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "Body";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Subject";
            // 
            // attachCamImgChkBox
            // 
            this.attachCamImgChkBox.AutoSize = true;
            this.attachCamImgChkBox.Location = new System.Drawing.Point(6, 104);
            this.attachCamImgChkBox.Name = "attachCamImgChkBox";
            this.attachCamImgChkBox.Size = new System.Drawing.Size(161, 17);
            this.attachCamImgChkBox.TabIndex = 4;
            this.attachCamImgChkBox.Text = "Attach related camera image";
            this.attachCamImgChkBox.UseVisualStyleBackColor = true;
            // 
            // alarmOffsetNum
            // 
            this.alarmOffsetNum.DecimalPlaces = 1;
            this.alarmOffsetNum.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.alarmOffsetNum.Location = new System.Drawing.Point(6, 140);
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
            this.alarmOffsetNum.Size = new System.Drawing.Size(58, 20);
            this.alarmOffsetNum.TabIndex = 5;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 124);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(110, 13);
            this.label5.TabIndex = 18;
            this.label5.Text = "Alarm time offset (sec)";
            // 
            // cooldownNum
            // 
            this.cooldownNum.Location = new System.Drawing.Point(6, 398);
            this.cooldownNum.Maximum = new decimal(new int[] {
            1440,
            0,
            0,
            0});
            this.cooldownNum.Name = "cooldownNum";
            this.cooldownNum.Size = new System.Drawing.Size(39, 20);
            this.cooldownNum.TabIndex = 19;
            this.cooldownNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 382);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(79, 13);
            this.label6.TabIndex = 20;
            this.label6.Text = "Cooldown (min)";
            // 
            // enabledChkBox
            // 
            this.enabledChkBox.AutoSize = true;
            this.enabledChkBox.Checked = true;
            this.enabledChkBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.enabledChkBox.Location = new System.Drawing.Point(3, 3);
            this.enabledChkBox.Name = "enabledChkBox";
            this.enabledChkBox.Size = new System.Drawing.Size(65, 17);
            this.enabledChkBox.TabIndex = 1;
            this.enabledChkBox.Text = "Enabled";
            this.enabledChkBox.UseVisualStyleBackColor = true;
            // 
            // EmailRecipientNodeEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.enabledChkBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cooldownNum);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.alarmOffsetNum);
            this.Controls.Add(this.attachCamImgChkBox);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.emailTxtBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.nameTxtBox);
            this.Controls.Add(this.label1);
            this.Name = "EmailRecipientNodeEditor";
            this.Size = new System.Drawing.Size(366, 469);
            this.Load += new System.EventHandler(this.EmailRecipientNodeEditor_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.alarmOffsetNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cooldownNum)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox formattingTxtBox;
        private System.Windows.Forms.TextBox emailTxtBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox nameTxtBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox subjectTxtBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox attachCamImgChkBox;
        private System.Windows.Forms.NumericUpDown alarmOffsetNum;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown cooldownNum;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox enabledChkBox;
    }
}
