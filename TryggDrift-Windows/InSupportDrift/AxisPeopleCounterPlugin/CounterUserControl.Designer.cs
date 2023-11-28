namespace AxisPeopleCounterPlugin
{
    partial class CounterUserControl
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
            this.userTxtBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.pwdTxtBox = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.nameTxtBox = new System.Windows.Forms.TextBox();
            this.closeBtn = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ipTxtBox
            // 
            this.ipTxtBox.Location = new System.Drawing.Point(44, 32);
            this.ipTxtBox.Name = "ipTxtBox";
            this.ipTxtBox.Size = new System.Drawing.Size(181, 20);
            this.ipTxtBox.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "IP";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "User";
            // 
            // userTxtBox
            // 
            this.userTxtBox.Location = new System.Drawing.Point(44, 58);
            this.userTxtBox.Name = "userTxtBox";
            this.userTxtBox.Size = new System.Drawing.Size(181, 20);
            this.userTxtBox.TabIndex = 3;
            this.userTxtBox.Text = "root";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 87);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(28, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Pwd";
            // 
            // pwdTxtBox
            // 
            this.pwdTxtBox.Location = new System.Drawing.Point(44, 84);
            this.pwdTxtBox.Name = "pwdTxtBox";
            this.pwdTxtBox.PasswordChar = '*';
            this.pwdTxtBox.Size = new System.Drawing.Size(181, 20);
            this.pwdTxtBox.TabIndex = 4;
            this.pwdTxtBox.Text = "pass";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.nameTxtBox);
            this.panel1.Controls.Add(this.closeBtn);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.ipTxtBox);
            this.panel1.Controls.Add(this.pwdTxtBox);
            this.panel1.Controls.Add(this.userTxtBox);
            this.panel1.Controls.Add(this.label2);
            this.panel1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(254, 113);
            this.panel1.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Name";
            // 
            // nameTxtBox
            // 
            this.nameTxtBox.Location = new System.Drawing.Point(44, 6);
            this.nameTxtBox.Name = "nameTxtBox";
            this.nameTxtBox.Size = new System.Drawing.Size(181, 20);
            this.nameTxtBox.TabIndex = 1;
            // 
            // closeBtn
            // 
            this.closeBtn.Location = new System.Drawing.Point(231, 2);
            this.closeBtn.Name = "closeBtn";
            this.closeBtn.Size = new System.Drawing.Size(20, 20);
            this.closeBtn.TabIndex = 5;
            this.closeBtn.Text = "X";
            this.closeBtn.UseVisualStyleBackColor = true;
            this.closeBtn.Click += new System.EventHandler(this.CloseBtn_Click);
            // 
            // CounterUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DimGray;
            this.Controls.Add(this.panel1);
            this.Name = "CounterUserControl";
            this.Size = new System.Drawing.Size(260, 119);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox ipTxtBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox userTxtBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox pwdTxtBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button closeBtn;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox nameTxtBox;
    }
}
