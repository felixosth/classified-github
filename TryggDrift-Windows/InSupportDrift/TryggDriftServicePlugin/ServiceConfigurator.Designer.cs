namespace InSupportDrift
{
    partial class ServiceConfigurator
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
            this.servicesListBox = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.searchBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.selectedServicesListBox = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // servicesListBox
            // 
            this.servicesListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.servicesListBox.FormattingEnabled = true;
            this.servicesListBox.Location = new System.Drawing.Point(3, 63);
            this.servicesListBox.Name = "servicesListBox";
            this.servicesListBox.Size = new System.Drawing.Size(270, 316);
            this.servicesListBox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(128, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select services to monitor";
            // 
            // searchBox
            // 
            this.searchBox.Location = new System.Drawing.Point(53, 37);
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(220, 20);
            this.searchBox.TabIndex = 2;
            this.searchBox.TextChanged += new System.EventHandler(this.searchBox_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Search:";
            // 
            // selectedServicesListBox
            // 
            this.selectedServicesListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.selectedServicesListBox.FormattingEnabled = true;
            this.selectedServicesListBox.Location = new System.Drawing.Point(279, 63);
            this.selectedServicesListBox.Name = "selectedServicesListBox";
            this.selectedServicesListBox.Size = new System.Drawing.Size(246, 316);
            this.selectedServicesListBox.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(279, 40);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(91, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Selected services";
            // 
            // ServiceConfigurator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.selectedServicesListBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.searchBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.servicesListBox);
            this.Name = "ServiceConfigurator";
            this.Size = new System.Drawing.Size(528, 388);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox servicesListBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox searchBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox selectedServicesListBox;
        private System.Windows.Forms.Label label3;
    }
}
