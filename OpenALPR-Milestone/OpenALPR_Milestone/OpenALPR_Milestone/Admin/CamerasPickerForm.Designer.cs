namespace OpenALPR_Milestone.Admin
{
    partial class CamerasPickerForm
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
            this.itemPickerContainer = new System.Windows.Forms.Panel();
            this.okBtn = new System.Windows.Forms.Button();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // itemPickerContainer
            // 
            this.itemPickerContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.itemPickerContainer.Location = new System.Drawing.Point(12, 12);
            this.itemPickerContainer.Name = "itemPickerContainer";
            this.itemPickerContainer.Size = new System.Drawing.Size(776, 397);
            this.itemPickerContainer.TabIndex = 0;
            // 
            // okBtn
            // 
            this.okBtn.Location = new System.Drawing.Point(632, 415);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(75, 23);
            this.okBtn.TabIndex = 1;
            this.okBtn.Text = "OK";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.OkBtn_Click);
            // 
            // cancelBtn
            // 
            this.cancelBtn.Location = new System.Drawing.Point(713, 415);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(75, 23);
            this.cancelBtn.TabIndex = 2;
            this.cancelBtn.Text = "Cancel";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // CamerasPickerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.okBtn);
            this.Controls.Add(this.itemPickerContainer);
            this.Name = "CamerasPickerForm";
            this.Text = "CamerasPickerForm";
            this.Load += new System.EventHandler(this.CamerasPickerForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel itemPickerContainer;
        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.Button cancelBtn;
    }
}