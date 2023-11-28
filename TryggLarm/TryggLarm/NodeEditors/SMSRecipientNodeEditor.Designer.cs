namespace TryggLarm.NodeEditors
{
    partial class SMSRecipientNodeEditor
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
            this.nameTxtBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.telTxtBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.formattingTxtBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cooldownNum = new System.Windows.Forms.NumericUpDown();
            this.enabledChkBox = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.cooldownNum)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // nameTxtBox
            // 
            this.nameTxtBox.Location = new System.Drawing.Point(6, 55);
            this.nameTxtBox.Name = "nameTxtBox";
            this.nameTxtBox.Size = new System.Drawing.Size(281, 20);
            this.nameTxtBox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 99;
            this.label1.Text = "Name";
            // 
            // telTxtBox
            // 
            this.telTxtBox.Location = new System.Drawing.Point(6, 94);
            this.telTxtBox.Name = "telTxtBox";
            this.telTxtBox.Size = new System.Drawing.Size(281, 20);
            this.telTxtBox.TabIndex = 2;
            //this.telTxtBox.TextChanged += 
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(22, 13);
            this.label2.TabIndex = 43;
            this.label2.Text = "Tel";
            // 
            // formattingTxtBox
            // 
            this.formattingTxtBox.Location = new System.Drawing.Point(6, 133);
            this.formattingTxtBox.Multiline = true;
            this.formattingTxtBox.Name = "formattingTxtBox";
            this.formattingTxtBox.Size = new System.Drawing.Size(281, 138);
            this.formattingTxtBox.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 117);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Formatting";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 274);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(79, 13);
            this.label6.TabIndex = 22;
            this.label6.Text = "Cooldown (min)";
            // 
            // cooldownNum
            // 
            this.cooldownNum.Location = new System.Drawing.Point(9, 290);
            this.cooldownNum.Maximum = new decimal(new int[] {
            1440,
            0,
            0,
            0});
            this.cooldownNum.Name = "cooldownNum";
            this.cooldownNum.Size = new System.Drawing.Size(39, 20);
            this.cooldownNum.TabIndex = 4;
            this.cooldownNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // enabledChkBox
            // 
            this.enabledChkBox.AutoSize = true;
            this.enabledChkBox.Checked = true;
            this.enabledChkBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.enabledChkBox.Location = new System.Drawing.Point(6, 19);
            this.enabledChkBox.Name = "enabledChkBox";
            this.enabledChkBox.Size = new System.Drawing.Size(65, 17);
            this.enabledChkBox.TabIndex = 0;
            this.enabledChkBox.Text = "Enabled";
            this.enabledChkBox.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.enabledChkBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.nameTxtBox);
            this.groupBox1.Controls.Add(this.cooldownNum);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.formattingTxtBox);
            this.groupBox1.Controls.Add(this.telTxtBox);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(558, 416);
            this.groupBox1.TabIndex = 24;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "SMS Recipient";
            // 
            // SMSRecipientNodeEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "SMSRecipientNodeEditor";
            this.Size = new System.Drawing.Size(564, 422);
            this.Load += new System.EventHandler(this.SMSRecipientNodeEditor_Load);
            ((System.ComponentModel.ISupportInitialize)(this.cooldownNum)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox nameTxtBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox telTxtBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox formattingTxtBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown cooldownNum;
        private System.Windows.Forms.CheckBox enabledChkBox;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}
