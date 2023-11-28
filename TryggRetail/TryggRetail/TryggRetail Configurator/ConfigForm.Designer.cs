namespace TryggRetail_Configurator
{
    partial class ConfigForm
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
            this.itemsListBox = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.saveItemBtn = new System.Windows.Forms.Button();
            this.refreshListBtn = new System.Windows.Forms.Button();
            this.deleteItemBtn = new System.Windows.Forms.Button();
            this.newItemBtn = new System.Windows.Forms.Button();
            this.licBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // itemsListBox
            // 
            this.itemsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.itemsListBox.FormattingEnabled = true;
            this.itemsListBox.Location = new System.Drawing.Point(12, 12);
            this.itemsListBox.Name = "itemsListBox";
            this.itemsListBox.Size = new System.Drawing.Size(155, 446);
            this.itemsListBox.TabIndex = 0;
            this.itemsListBox.SelectedIndexChanged += new System.EventHandler(this.itemsListBox_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel1.Location = new System.Drawing.Point(173, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(644, 446);
            this.panel1.TabIndex = 1;
            // 
            // saveItemBtn
            // 
            this.saveItemBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveItemBtn.Location = new System.Drawing.Point(742, 468);
            this.saveItemBtn.Name = "saveItemBtn";
            this.saveItemBtn.Size = new System.Drawing.Size(75, 23);
            this.saveItemBtn.TabIndex = 2;
            this.saveItemBtn.Text = "Save";
            this.saveItemBtn.UseVisualStyleBackColor = true;
            this.saveItemBtn.Click += new System.EventHandler(this.saveBtn_Click);
            // 
            // refreshListBtn
            // 
            this.refreshListBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.refreshListBtn.Location = new System.Drawing.Point(12, 468);
            this.refreshListBtn.Name = "refreshListBtn";
            this.refreshListBtn.Size = new System.Drawing.Size(75, 23);
            this.refreshListBtn.TabIndex = 3;
            this.refreshListBtn.Text = "Refresh";
            this.refreshListBtn.UseVisualStyleBackColor = true;
            this.refreshListBtn.Click += new System.EventHandler(this.refreshListBtn_Click);
            // 
            // deleteItemBtn
            // 
            this.deleteItemBtn.Location = new System.Drawing.Point(661, 468);
            this.deleteItemBtn.Name = "deleteItemBtn";
            this.deleteItemBtn.Size = new System.Drawing.Size(75, 23);
            this.deleteItemBtn.TabIndex = 4;
            this.deleteItemBtn.Text = "Delete";
            this.deleteItemBtn.UseVisualStyleBackColor = true;
            this.deleteItemBtn.Click += new System.EventHandler(this.deleteItemBtn_Click);
            // 
            // newItemBtn
            // 
            this.newItemBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.newItemBtn.Location = new System.Drawing.Point(92, 468);
            this.newItemBtn.Name = "newItemBtn";
            this.newItemBtn.Size = new System.Drawing.Size(75, 23);
            this.newItemBtn.TabIndex = 5;
            this.newItemBtn.Text = "New";
            this.newItemBtn.UseVisualStyleBackColor = true;
            this.newItemBtn.Click += new System.EventHandler(this.newItemBtn_Click);
            // 
            // licBtn
            // 
            this.licBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.licBtn.Location = new System.Drawing.Point(173, 468);
            this.licBtn.Name = "licBtn";
            this.licBtn.Size = new System.Drawing.Size(137, 23);
            this.licBtn.TabIndex = 6;
            this.licBtn.Text = "License Management";
            this.licBtn.UseVisualStyleBackColor = true;
            this.licBtn.Click += new System.EventHandler(this.licBtn_Click);
            // 
            // ConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(829, 503);
            this.Controls.Add(this.licBtn);
            this.Controls.Add(this.newItemBtn);
            this.Controls.Add(this.deleteItemBtn);
            this.Controls.Add(this.refreshListBtn);
            this.Controls.Add(this.saveItemBtn);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.itemsListBox);
            this.Name = "ConfigForm";
            this.Text = "TryggRetail Configurator";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConfigForm_FormClosing);
            this.Load += new System.EventHandler(this.ConfigForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox itemsListBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button saveItemBtn;
        private System.Windows.Forms.Button refreshListBtn;
        private System.Windows.Forms.Button deleteItemBtn;
        private System.Windows.Forms.Button newItemBtn;
        private System.Windows.Forms.Button licBtn;
    }
}

