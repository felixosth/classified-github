
namespace RegSormlandMilestonePlugin.Admin
{
    partial class AddEventToCameraForm
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
            this.cameraTxtBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.eventTxtBox = new System.Windows.Forms.TextBox();
            this.browseEventBtn = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.timeToDisplayNumVal = new System.Windows.Forms.NumericUpDown();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.okBtn = new System.Windows.Forms.Button();
            this.browseBtnEventBtn = new System.Windows.Forms.Button();
            this.btnEventTxtBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.buttonTextTxtBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.timeToDisplayNumVal)).BeginInit();
            this.SuspendLayout();
            // 
            // cameraTxtBox
            // 
            this.cameraTxtBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cameraTxtBox.Location = new System.Drawing.Point(95, 6);
            this.cameraTxtBox.Name = "cameraTxtBox";
            this.cameraTxtBox.ReadOnly = true;
            this.cameraTxtBox.Size = new System.Drawing.Size(436, 20);
            this.cameraTxtBox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Camera";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Event";
            // 
            // eventTxtBox
            // 
            this.eventTxtBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.eventTxtBox.Location = new System.Drawing.Point(95, 40);
            this.eventTxtBox.Name = "eventTxtBox";
            this.eventTxtBox.ReadOnly = true;
            this.eventTxtBox.Size = new System.Drawing.Size(385, 20);
            this.eventTxtBox.TabIndex = 3;
            // 
            // browseEventBtn
            // 
            this.browseEventBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseEventBtn.Location = new System.Drawing.Point(486, 38);
            this.browseEventBtn.Name = "browseEventBtn";
            this.browseEventBtn.Size = new System.Drawing.Size(45, 23);
            this.browseEventBtn.TabIndex = 4;
            this.browseEventBtn.Text = "...";
            this.browseEventBtn.UseVisualStyleBackColor = true;
            this.browseEventBtn.Click += new System.EventHandler(this.browseEventBtn_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 121);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Time to display";
            // 
            // timeToDisplayNumVal
            // 
            this.timeToDisplayNumVal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.timeToDisplayNumVal.Location = new System.Drawing.Point(95, 119);
            this.timeToDisplayNumVal.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.timeToDisplayNumVal.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.timeToDisplayNumVal.Name = "timeToDisplayNumVal";
            this.timeToDisplayNumVal.Size = new System.Drawing.Size(436, 20);
            this.timeToDisplayNumVal.TabIndex = 6;
            this.timeToDisplayNumVal.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // cancelBtn
            // 
            this.cancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelBtn.Location = new System.Drawing.Point(456, 159);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(75, 23);
            this.cancelBtn.TabIndex = 7;
            this.cancelBtn.Text = "Cancel";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // okBtn
            // 
            this.okBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okBtn.Location = new System.Drawing.Point(375, 159);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(75, 23);
            this.okBtn.TabIndex = 8;
            this.okBtn.Text = "OK";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
            // 
            // browseBtnEventBtn
            // 
            this.browseBtnEventBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseBtnEventBtn.Location = new System.Drawing.Point(486, 64);
            this.browseBtnEventBtn.Name = "browseBtnEventBtn";
            this.browseBtnEventBtn.Size = new System.Drawing.Size(45, 23);
            this.browseBtnEventBtn.TabIndex = 11;
            this.browseBtnEventBtn.Text = "...";
            this.browseBtnEventBtn.UseVisualStyleBackColor = true;
            this.browseBtnEventBtn.Click += new System.EventHandler(this.browseBtnEventBtn_Click);
            // 
            // btnEventTxtBox
            // 
            this.btnEventTxtBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEventTxtBox.Location = new System.Drawing.Point(95, 66);
            this.btnEventTxtBox.Name = "btnEventTxtBox";
            this.btnEventTxtBox.ReadOnly = true;
            this.btnEventTxtBox.Size = new System.Drawing.Size(385, 20);
            this.btnEventTxtBox.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 69);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Event (Button)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 96);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Button text";
            // 
            // buttonTextTxtBox
            // 
            this.buttonTextTxtBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonTextTxtBox.Location = new System.Drawing.Point(95, 93);
            this.buttonTextTxtBox.Name = "buttonTextTxtBox";
            this.buttonTextTxtBox.Size = new System.Drawing.Size(436, 20);
            this.buttonTextTxtBox.TabIndex = 12;
            // 
            // AddEventToCameraForm
            // 
            this.AcceptButton = this.okBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelBtn;
            this.ClientSize = new System.Drawing.Size(543, 194);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.buttonTextTxtBox);
            this.Controls.Add(this.browseBtnEventBtn);
            this.Controls.Add(this.btnEventTxtBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.okBtn);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.timeToDisplayNumVal);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.browseEventBtn);
            this.Controls.Add(this.eventTxtBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cameraTxtBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddEventToCameraForm";
            this.Text = "AddEventToCameraForm";
            ((System.ComponentModel.ISupportInitialize)(this.timeToDisplayNumVal)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button browseEventBtn;
        private System.Windows.Forms.Label label3;
        internal System.Windows.Forms.TextBox cameraTxtBox;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.Button okBtn;
        internal System.Windows.Forms.TextBox eventTxtBox;
        internal System.Windows.Forms.NumericUpDown timeToDisplayNumVal;
        private System.Windows.Forms.Button browseBtnEventBtn;
        internal System.Windows.Forms.TextBox btnEventTxtBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        internal System.Windows.Forms.TextBox buttonTextTxtBox;
    }
}