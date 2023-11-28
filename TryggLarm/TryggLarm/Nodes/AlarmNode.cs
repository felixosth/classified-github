using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using TryggLarm.Admin;

namespace TryggLarm.Nodes
{
    [Serializable]
    class AlarmNode : CustomNode
    {
        public AlarmNode(string name) : base(name) { }

        public AlarmNode(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override void Initialize()
        {
            ContextMenu = new System.Windows.Forms.ContextMenu(new System.Windows.Forms.MenuItem[]
            {
                new System.Windows.Forms.MenuItem("Remove alarm", RemoveNodeClick)
            });

            base.Initialize();
        }

        void RemoveNodeClick(object sender, EventArgs e)
        {
            NodeMng.NodeTreeView.BeginUpdate();
            NodeMng.NodeTreeView.Nodes.Remove(this);
            NodeMng.NodeTreeView.EndUpdate();

            NodeMng.OnUserChange(sender, e);

            NodeMng.ClearSettingsPanel();
        }

        public override void InitializeEditor()
        {
            myNodeEditor = new NodeEditors.NameNodeEditor(this.NodeMng);
            base.InitializeEditor();
        }
    }
}
