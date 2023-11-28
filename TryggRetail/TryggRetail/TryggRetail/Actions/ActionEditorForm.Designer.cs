namespace TryggRetail.Actions
{
    partial class ActionEditorForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.actionTypeComboBox = new System.Windows.Forms.ComboBox();
            this.customActionEditorContainer = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.displayTextBox = new System.Windows.Forms.TextBox();
            this.okBtn = new System.Windows.Forms.Button();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnTxtColDisplay = new System.Windows.Forms.PictureBox();
            this.btnBackColDisplay = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.btnTxtColDisplay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnBackColDisplay)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Action type:";
            // 
            // actionTypeComboBox
            // 
            this.actionTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.actionTypeComboBox.FormattingEnabled = true;
            this.actionTypeComboBox.Items.AddRange(new object[] {
            "HTTP Request (e.g. Vapix)",
            "Trigger Event"});
            this.actionTypeComboBox.Location = new System.Drawing.Point(81, 6);
            this.actionTypeComboBox.Name = "actionTypeComboBox";
            this.actionTypeComboBox.Size = new System.Drawing.Size(170, 21);
            this.actionTypeComboBox.TabIndex = 1;
            this.actionTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.actionTypeComboBox_SelectedIndexChanged);
            // 
            // customActionEditorContainer
            // 
            this.customActionEditorContainer.Location = new System.Drawing.Point(15, 62);
            this.customActionEditorContainer.Name = "customActionEditorContainer";
            this.customActionEditorContainer.Size = new System.Drawing.Size(387, 165);
            this.customActionEditorContainer.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Display text:";
            // 
            // displayTextBox
            // 
            this.displayTextBox.Location = new System.Drawing.Point(82, 36);
            this.displayTextBox.Name = "displayTextBox";
            this.displayTextBox.Size = new System.Drawing.Size(170, 20);
            this.displayTextBox.TabIndex = 4;
            this.displayTextBox.Text = "Custom action";
            // 
            // okBtn
            // 
            this.okBtn.Location = new System.Drawing.Point(327, 233);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(75, 23);
            this.okBtn.TabIndex = 5;
            this.okBtn.Text = "OK";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(257, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Button text color:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(258, 39);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(94, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Button back color:";
            // 
            // btnTxtColDisplay
            // 
            this.btnTxtColDisplay.BackColor = System.Drawing.Color.Black;
            this.btnTxtColDisplay.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTxtColDisplay.Location = new System.Drawing.Point(358, 6);
            this.btnTxtColDisplay.Name = "btnTxtColDisplay";
            this.btnTxtColDisplay.Size = new System.Drawing.Size(44, 21);
            this.btnTxtColDisplay.TabIndex = 8;
            this.btnTxtColDisplay.TabStop = false;
            this.btnTxtColDisplay.Click += new System.EventHandler(this.btnTxtColDisplay_Click);
            // 
            // btnBackColDisplay
            // 
            this.btnBackColDisplay.BackColor = System.Drawing.Color.Violet;
            this.btnBackColDisplay.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBackColDisplay.Location = new System.Drawing.Point(358, 36);
            this.btnBackColDisplay.Name = "btnBackColDisplay";
            this.btnBackColDisplay.Size = new System.Drawing.Size(44, 20);
            this.btnBackColDisplay.TabIndex = 9;
            this.btnBackColDisplay.TabStop = false;
            this.btnBackColDisplay.Click += new System.EventHandler(this.btnBackColDisplay_Click);
            // 
            // ActionEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(411, 264);
            this.Controls.Add(this.btnBackColDisplay);
            this.Controls.Add(this.btnTxtColDisplay);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.okBtn);
            this.Controls.Add(this.displayTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.customActionEditorContainer);
            this.Controls.Add(this.actionTypeComboBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "ActionEditorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ActionEditorForm";
            this.Load += new System.EventHandler(this.ActionEditorForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.btnTxtColDisplay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnBackColDisplay)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox actionTypeComboBox;
        private System.Windows.Forms.Panel customActionEditorContainer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox displayTextBox;
        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox btnTxtColDisplay;
        private System.Windows.Forms.PictureBox btnBackColDisplay;
    }
}