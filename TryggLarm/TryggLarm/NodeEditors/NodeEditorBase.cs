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
    public partial class NodeEditorBase : UserControl
    {
        protected CustomNode myNode;
        protected NodeEditorManager nodeEditorMng;

        public NodeEditorBase()
        {
            InitializeComponent();
        }

        public NodeEditorBase(NodeEditorManager mngInstance)
        {
            InitializeComponent();
            nodeEditorMng = mngInstance;
        }
        


        private void NodeEditorBase_Load(object sender, EventArgs e)
        {

        }

        public virtual void LoadValues(CustomNode node)
        {
            myNode = node;
        }

        public virtual void SetValues()
        {
        }


        public bool ValuesAreModified = false;
        public virtual void ModifiedValues(object sender, EventArgs e)
        {
            ValuesAreModified = true;

           nodeEditorMng.OnUserChange(sender, e);
        }
    }
}
