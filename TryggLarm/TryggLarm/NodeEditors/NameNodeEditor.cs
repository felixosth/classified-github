using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TryggLarm.Nodes;
using TryggLarm.Admin;

namespace TryggLarm.NodeEditors
{
    public partial class NameNodeEditor : NodeEditorBase
    {
        public NameNodeEditor(NodeEditorManager nodeMng) : base(nodeMng)
        {
            InitializeComponent();
        }

        //public NameNodeEditor()
        //{
        //}

        private void RootNodeEditor_Load(object sender, EventArgs e)
        {
        }

        public override void ModifiedValues(object sender, EventArgs e)
        {
            SetValues();
            base.ModifiedValues(sender, e);
        }

        public override void LoadValues(CustomNode node)
        {
            //var rootNode = node as RootNode;
            nameTxtBox.Text = node.DisplayName;


            nameTxtBox.TextChanged += ModifiedValues;

            base.LoadValues(node);
        }

        public override void SetValues()
        {
            //var rootNode = myNode as RootNode;
            myNode.DisplayName = nameTxtBox.Text;

            base.SetValues();
        }
    }
}
