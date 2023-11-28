using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TryggLarm.Admin;
using TryggLarm.NodeEditors;

namespace TryggLarm.Nodes
{
    [Serializable]
    public class RootNode : CustomNode
    {

        public RootNode(string name) : base(name)
        {
        }

        public RootNode(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        public override void Initialize()
        {
            ContextMenu = new ContextMenu(new MenuItem[]
            {
                new MenuItem("New object", AddNewClick)
            });

            base.Initialize();
        }

        void AddNewClick(object sender, EventArgs e)
        {
            var newNode = new AlarmObjectNode("New alarm object")
            {
                NodeMng = this.NodeMng
            };
            newNode.OnNodeCreated(NodeMng);

            NodeMng.NodeTreeView.BeginUpdate();
            this.Nodes.Add(newNode);
            NodeMng.NodeTreeView.EndUpdate();
            this.Expand();

            NodeMng.NodeTreeView.SelectedNode = newNode;

            NodeMng.OnUserChange(this, e);
            //newNode.BeginEdit();
        }

        public override void InitializeEditor()
        {
            myNodeEditor = new NameNodeEditor(this.NodeMng);
            //myNodeEditor.LoadValues(this);

            base.InitializeEditor();
        }

    }
}
