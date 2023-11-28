namespace TryggLarm.Admin
{
    partial class NodeEditorManager
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Object name here");
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.NodeTreeView = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.NodeTreeView);
            this.splitContainer.Size = new System.Drawing.Size(919, 561);
            this.splitContainer.SplitterDistance = 222;
            this.splitContainer.TabIndex = 1;
            // 
            // NodeTreeView
            // 
            this.NodeTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NodeTreeView.Location = new System.Drawing.Point(0, 0);
            this.NodeTreeView.Name = "NodeTreeView";
            treeNode1.Name = "RootNode";
            treeNode1.Text = "Object name here";
            this.NodeTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.NodeTreeView.Size = new System.Drawing.Size(222, 561);
            this.NodeTreeView.TabIndex = 0;
            this.NodeTreeView.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.NodeView_BeforeLabelEdit);
            this.NodeTreeView.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.NodeTreeView_AfterLabelEdit);
            this.NodeTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.nodeView_AfterSelect);
            this.NodeTreeView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.NodeTreeView_KeyDown);
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // NodeEditorManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer);
            this.Name = "NodeEditorManager";
            this.Size = new System.Drawing.Size(919, 561);
            this.Load += new System.EventHandler(this.NodeEditorManager_Load);
            this.splitContainer.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.SplitContainer splitContainer;
        public System.Windows.Forms.TreeView NodeTreeView;
        private System.Windows.Forms.ImageList imageList1;
    }
}
