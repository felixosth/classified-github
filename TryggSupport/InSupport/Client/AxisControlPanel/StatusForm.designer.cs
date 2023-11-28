namespace InSupport
{
    partial class StatusViewer
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StatusViewer));
            this.buttonClose = new System.Windows.Forms.Button();
            this.treeViewItems = new System.Windows.Forms.TreeView();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.listBoxChanges = new System.Windows.Forms.ListBox();
            this.labelConnected = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.Location = new System.Drawing.Point(473, 442);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 0;
            this.buttonClose.Text = "Stäng";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.OnClose);
            // 
            // treeViewItems
            // 
            this.treeViewItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewItems.Location = new System.Drawing.Point(12, 28);
            this.treeViewItems.Name = "treeViewItems";
            this.treeViewItems.Size = new System.Drawing.Size(536, 203);
            this.treeViewItems.TabIndex = 1;
            // 
            // listBoxChanges
            // 
            this.listBoxChanges.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxChanges.FormattingEnabled = true;
            this.listBoxChanges.Location = new System.Drawing.Point(12, 237);
            this.listBoxChanges.Name = "listBoxChanges";
            this.listBoxChanges.Size = new System.Drawing.Size(536, 199);
            this.listBoxChanges.TabIndex = 3;
            // 
            // labelConnected
            // 
            this.labelConnected.AutoSize = true;
            this.labelConnected.BackColor = System.Drawing.Color.Transparent;
            this.labelConnected.Location = new System.Drawing.Point(12, 9);
            this.labelConnected.Name = "labelConnected";
            this.labelConnected.Size = new System.Drawing.Size(37, 13);
            this.labelConnected.TabIndex = 4;
            this.labelConnected.Text = "Status";
            // 
            // StatusViewer
            // 
            this.AcceptButton = this.buttonClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(560, 477);
            this.Controls.Add(this.labelConnected);
            this.Controls.Add(this.listBoxChanges);
            this.Controls.Add(this.treeViewItems);
            this.Controls.Add(this.buttonClose);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "StatusViewer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "StatusViewer";
            this.Load += new System.EventHandler(this.OnLoad);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.TreeView treeViewItems;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ListBox listBoxChanges;
        private System.Windows.Forms.Label labelConnected;
    }
}