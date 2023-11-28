namespace LicenseTester
{
    partial class Form1
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.monthCalendar1 = new System.Windows.Forms.MonthCalendar();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.lStatusLabel = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.activateLicenseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.activateLicenseToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshLicenseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.monthCalendar1);
            this.groupBox1.Controls.Add(this.radioButton2);
            this.groupBox1.Controls.Add(this.radioButton1);
            this.groupBox1.Controls.Add(this.checkBox1);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Location = new System.Drawing.Point(12, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(396, 244);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Features behind License";
            // 
            // monthCalendar1
            // 
            this.monthCalendar1.Location = new System.Drawing.Point(170, 70);
            this.monthCalendar1.Name = "monthCalendar1";
            this.monthCalendar1.TabIndex = 5;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Checked = true;
            this.radioButton2.Location = new System.Drawing.Point(254, 45);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(65, 17);
            this.radioButton2.TabIndex = 4;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "Option 1";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(325, 45);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(65, 17);
            this.radioButton1.TabIndex = 3;
            this.radioButton1.Text = "Option 2";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(6, 19);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(123, 17);
            this.checkBox1.TabIndex = 2;
            this.checkBox1.Text = "Important checkbox!";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(6, 42);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(139, 20);
            this.textBox1.TabIndex = 1;
            this.textBox1.Text = "lots of text!";
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(6, 68);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(139, 72);
            this.button1.TabIndex = 0;
            this.button1.Text = "Do stuff!";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lStatusLabel
            // 
            this.lStatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lStatusLabel.AutoSize = true;
            this.lStatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lStatusLabel.Location = new System.Drawing.Point(9, 274);
            this.lStatusLabel.Name = "lStatusLabel";
            this.lStatusLabel.Size = new System.Drawing.Size(77, 16);
            this.lStatusLabel.TabIndex = 0;
            this.lStatusLabel.Text = "statusLabel";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.activateLicenseToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(420, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // activateLicenseToolStripMenuItem
            // 
            this.activateLicenseToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.activateLicenseToolStripMenuItem1,
            this.refreshLicenseToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.activateLicenseToolStripMenuItem.Name = "activateLicenseToolStripMenuItem";
            this.activateLicenseToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            this.activateLicenseToolStripMenuItem.Text = "License";
            this.activateLicenseToolStripMenuItem.Click += new System.EventHandler(this.activateLicenseToolStripMenuItem_Click);
            // 
            // activateLicenseToolStripMenuItem1
            // 
            this.activateLicenseToolStripMenuItem1.Name = "activateLicenseToolStripMenuItem1";
            this.activateLicenseToolStripMenuItem1.Size = new System.Drawing.Size(162, 22);
            this.activateLicenseToolStripMenuItem1.Text = "Activate license..";
            this.activateLicenseToolStripMenuItem1.Click += new System.EventHandler(this.activateLicenseToolStripMenuItem1_Click);
            // 
            // refreshLicenseToolStripMenuItem
            // 
            this.refreshLicenseToolStripMenuItem.Name = "refreshLicenseToolStripMenuItem";
            this.refreshLicenseToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.refreshLicenseToolStripMenuItem.Text = "Refresh license";
            this.refreshLicenseToolStripMenuItem.Click += new System.EventHandler(this.refreshLicenseToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.aboutToolStripMenuItem.Text = "About..";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(420, 300);
            this.Controls.Add(this.lStatusLabel);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "License Test";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label lStatusLabel;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem activateLicenseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem activateLicenseToolStripMenuItem1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ToolStripMenuItem refreshLicenseToolStripMenuItem;
        private System.Windows.Forms.MonthCalendar monthCalendar1;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
    }
}

