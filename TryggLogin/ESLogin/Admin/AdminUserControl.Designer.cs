namespace ESLogin.Admin
{
    partial class AdminUserControl
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
            this.components = new System.ComponentModel.Container();
            this.usersListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.usersListViewContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.editUserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.removeUserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.usersListViewContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // usersListView
            // 
            this.usersListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.usersListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.usersListView.ContextMenuStrip = this.usersListViewContextMenu;
            this.usersListView.FullRowSelect = true;
            this.usersListView.HideSelection = false;
            this.usersListView.Location = new System.Drawing.Point(3, 3);
            this.usersListView.MultiSelect = false;
            this.usersListView.Name = "usersListView";
            this.usersListView.Size = new System.Drawing.Size(1215, 793);
            this.usersListView.TabIndex = 0;
            this.usersListView.UseCompatibleStateImageBehavior = false;
            this.usersListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 140;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Authenticator";
            this.columnHeader2.Width = 209;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Type";
            // 
            // usersListViewContextMenu
            // 
            this.usersListViewContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.addGroupToolStripMenuItem,
            this.toolStripSeparator2,
            this.editUserToolStripMenuItem,
            this.toolStripSeparator1,
            this.removeUserToolStripMenuItem});
            this.usersListViewContextMenu.Name = "usersListViewContextMenu";
            this.usersListViewContextMenu.Size = new System.Drawing.Size(181, 126);
            this.usersListViewContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.usersListViewContextMenu_Opening);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.addToolStripMenuItem.Text = "Add user...";
            this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
            // 
            // addGroupToolStripMenuItem
            // 
            this.addGroupToolStripMenuItem.Name = "addGroupToolStripMenuItem";
            this.addGroupToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.addGroupToolStripMenuItem.Text = "Add group...";
            this.addGroupToolStripMenuItem.Click += new System.EventHandler(this.addGroupToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(177, 6);
            // 
            // editUserToolStripMenuItem
            // 
            this.editUserToolStripMenuItem.Name = "editUserToolStripMenuItem";
            this.editUserToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.editUserToolStripMenuItem.Text = "Edit...";
            this.editUserToolStripMenuItem.Click += new System.EventHandler(this.editUserToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
            // 
            // removeUserToolStripMenuItem
            // 
            this.removeUserToolStripMenuItem.Name = "removeUserToolStripMenuItem";
            this.removeUserToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.removeUserToolStripMenuItem.Text = "Remove user";
            this.removeUserToolStripMenuItem.Click += new System.EventHandler(this.removeUserToolStripMenuItem_Click);
            // 
            // AdminUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.usersListView);
            this.Name = "AdminUserControl";
            this.Size = new System.Drawing.Size(1221, 799);
            this.usersListViewContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView usersListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ContextMenuStrip usersListViewContextMenu;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeUserToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem editUserToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem addGroupToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader columnHeader3;
    }
}
