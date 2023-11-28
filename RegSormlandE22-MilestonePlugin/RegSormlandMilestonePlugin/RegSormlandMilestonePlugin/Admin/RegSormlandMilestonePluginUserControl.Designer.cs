
namespace RegSormlandMilestonePlugin.Admin
{
    partial class RegSormlandMilestonePluginUserControl
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
            this.camerasListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.camerasListViewContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeEventToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.camerasListViewContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // camerasListView
            // 
            this.camerasListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader4,
            this.columnHeader3});
            this.camerasListView.ContextMenuStrip = this.camerasListViewContextMenu;
            this.camerasListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.camerasListView.FullRowSelect = true;
            this.camerasListView.HideSelection = false;
            this.camerasListView.Location = new System.Drawing.Point(0, 0);
            this.camerasListView.MultiSelect = false;
            this.camerasListView.Name = "camerasListView";
            this.camerasListView.Size = new System.Drawing.Size(687, 673);
            this.camerasListView.TabIndex = 0;
            this.camerasListView.UseCompatibleStateImageBehavior = false;
            this.camerasListView.View = System.Windows.Forms.View.Details;
            this.camerasListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.camerasListView_MouseDoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Camera";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Event";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Time to display";
            this.columnHeader3.Width = 110;
            // 
            // camerasListViewContextMenu
            // 
            this.camerasListViewContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeEventToolStripMenuItem});
            this.camerasListViewContextMenu.Name = "camerasListViewContextMenu";
            this.camerasListViewContextMenu.Size = new System.Drawing.Size(150, 26);
            this.camerasListViewContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.camerasListViewContextMenu_Opening);
            // 
            // removeEventToolStripMenuItem
            // 
            this.removeEventToolStripMenuItem.Name = "removeEventToolStripMenuItem";
            this.removeEventToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.removeEventToolStripMenuItem.Text = "Remove event";
            this.removeEventToolStripMenuItem.Click += new System.EventHandler(this.removeEventToolStripMenuItem_Click);
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Button event";
            this.columnHeader4.Width = 89;
            // 
            // RegSormlandMilestonePluginUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.camerasListView);
            this.Name = "RegSormlandMilestonePluginUserControl";
            this.Size = new System.Drawing.Size(687, 673);
            this.camerasListViewContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView camerasListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ContextMenuStrip camerasListViewContextMenu;
        private System.Windows.Forms.ToolStripMenuItem removeEventToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
    }
}
