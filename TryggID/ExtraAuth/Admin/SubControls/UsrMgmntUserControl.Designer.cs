namespace TryggLogin.Admin.SubControls
{
    partial class UsrMgmntUserControl
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
            this.label2 = new System.Windows.Forms.Label();
            this.usersListBoxContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.usersListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.bankIdEnviromentComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.usersListBoxContextMenu.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(188, 25);
            this.label2.TabIndex = 2;
            this.label2.Text = "User Management";
            // 
            // usersListBoxContextMenu
            // 
            this.usersListBoxContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.usersListBoxContextMenu.Name = "usersListBoxContextMenu";
            this.usersListBoxContextMenu.Size = new System.Drawing.Size(108, 48);
            this.usersListBoxContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.usersListBoxContextMenu_Opening);
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.usersListView);
            this.groupBox1.Location = new System.Drawing.Point(8, 28);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(875, 568);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Users";
            // 
            // usersListView
            // 
            this.usersListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader4,
            this.columnHeader3});
            this.usersListView.ContextMenuStrip = this.usersListBoxContextMenu;
            this.usersListView.HideSelection = false;
            this.usersListView.Location = new System.Drawing.Point(6, 19);
            this.usersListView.Name = "usersListView";
            this.usersListView.Size = new System.Drawing.Size(684, 543);
            this.usersListView.TabIndex = 4;
            this.usersListView.UseCompatibleStateImageBehavior = false;
            this.usersListView.View = System.Windows.Forms.View.Details;
            this.usersListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.UsersListView_MouseDoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 257;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Role";
            this.columnHeader2.Width = 254;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Auth Type";
            this.columnHeader4.Width = 91;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Password";
            // 
            // bankIdEnviromentComboBox
            // 
            this.bankIdEnviromentComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.bankIdEnviromentComboBox.FormattingEnabled = true;
            this.bankIdEnviromentComboBox.Items.AddRange(new object[] {
            "test",
            "live"});
            this.bankIdEnviromentComboBox.Location = new System.Drawing.Point(83, 602);
            this.bankIdEnviromentComboBox.Name = "bankIdEnviromentComboBox";
            this.bankIdEnviromentComboBox.Size = new System.Drawing.Size(132, 21);
            this.bankIdEnviromentComboBox.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 605);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Environment";
            // 
            // UsrMgmntUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bankIdEnviromentComboBox);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label2);
            this.Name = "UsrMgmntUserControl";
            this.Size = new System.Drawing.Size(913, 676);
            this.usersListBoxContextMenu.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ContextMenuStrip usersListBoxContextMenu;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ListView usersListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ComboBox bankIdEnviromentComboBox;
        private System.Windows.Forms.Label label1;
    }
}
