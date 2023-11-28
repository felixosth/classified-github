namespace TryggDriftPingMonitorPlugin
{
    partial class PingItemUserControl
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
            this.ipTxtBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.typeTxtBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.labelTxtBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.deleteMeBtn = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ipTxtBox
            // 
            this.ipTxtBox.Location = new System.Drawing.Point(9, 32);
            this.ipTxtBox.Name = "ipTxtBox";
            this.ipTxtBox.Size = new System.Drawing.Size(141, 20);
            this.ipTxtBox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "IP";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(153, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Type";
            // 
            // typeTxtBox
            // 
            this.typeTxtBox.Location = new System.Drawing.Point(156, 32);
            this.typeTxtBox.Name = "typeTxtBox";
            this.typeTxtBox.Size = new System.Drawing.Size(141, 20);
            this.typeTxtBox.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(300, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Label";
            // 
            // labelTxtBox
            // 
            this.labelTxtBox.Location = new System.Drawing.Point(303, 32);
            this.labelTxtBox.Name = "labelTxtBox";
            this.labelTxtBox.Size = new System.Drawing.Size(141, 20);
            this.labelTxtBox.TabIndex = 2;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.ipTxtBox);
            this.groupBox1.Controls.Add(this.labelTxtBox);
            this.groupBox1.Controls.Add(this.typeTxtBox);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(452, 66);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Ping";
            // 
            // deleteMeBtn
            // 
            this.deleteMeBtn.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.deleteMeBtn.Location = new System.Drawing.Point(461, 28);
            this.deleteMeBtn.Name = "deleteMeBtn";
            this.deleteMeBtn.Size = new System.Drawing.Size(30, 23);
            this.deleteMeBtn.TabIndex = 7;
            this.deleteMeBtn.Text = "X";
            this.deleteMeBtn.UseVisualStyleBackColor = true;
            // 
            // PingItemUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.deleteMeBtn);
            this.Controls.Add(this.groupBox1);
            this.Name = "PingItemUserControl";
            this.Size = new System.Drawing.Size(499, 76);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox ipTxtBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox typeTxtBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox labelTxtBox;
        private System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.Button deleteMeBtn;
    }
}
