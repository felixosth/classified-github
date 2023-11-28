namespace TryggLarm.NodeEditors
{
    partial class NokasNodeEditor
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
            this.enabledChkBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.codeTxtBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.infoTxtBox = new System.Windows.Forms.TextBox();
            this.pinTxtBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.typeTxtBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.typeTxtBox);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.enabledChkBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.codeTxtBox);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.infoTxtBox);
            this.groupBox1.Controls.Add(this.pinTxtBox);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(490, 232);
            this.groupBox1.TabIndex = 25;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Nokas Alarm";
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 99;
            this.label1.Text = "Code";
            // 
            // codeTxtBox
            // 
            this.codeTxtBox.Location = new System.Drawing.Point(6, 55);
            this.codeTxtBox.Name = "codeTxtBox";
            this.codeTxtBox.Size = new System.Drawing.Size(281, 20);
            this.codeTxtBox.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(22, 13);
            this.label2.TabIndex = 43;
            this.label2.Text = "Pin";
            // 
            // infoTxtBox
            // 
            this.infoTxtBox.Location = new System.Drawing.Point(6, 133);
            this.infoTxtBox.Name = "infoTxtBox";
            this.infoTxtBox.Size = new System.Drawing.Size(281, 20);
            this.infoTxtBox.TabIndex = 3;
            this.infoTxtBox.TextChanged += new System.EventHandler(this.infoTxtBox_TextChanged);
            // 
            // pinTxtBox
            // 
            this.pinTxtBox.Location = new System.Drawing.Point(6, 94);
            this.pinTxtBox.Name = "pinTxtBox";
            this.pinTxtBox.Size = new System.Drawing.Size(281, 20);
            this.pinTxtBox.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 117);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(25, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Info";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // typeTxtBox
            // 
            this.typeTxtBox.Location = new System.Drawing.Point(6, 172);
            this.typeTxtBox.MaxLength = 2;
            this.typeTxtBox.Name = "typeTxtBox";
            this.typeTxtBox.Size = new System.Drawing.Size(281, 20);
            this.typeTxtBox.TabIndex = 100;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 156);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(31, 13);
            this.label4.TabIndex = 101;
            this.label4.Text = "Type";
            // 
            // NokasNodeEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "NokasNodeEditor";
            this.Size = new System.Drawing.Size(496, 238);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox enabledChkBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox codeTxtBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox infoTxtBox;
        private System.Windows.Forms.TextBox pinTxtBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox typeTxtBox;
        private System.Windows.Forms.Label label4;
    }
}
