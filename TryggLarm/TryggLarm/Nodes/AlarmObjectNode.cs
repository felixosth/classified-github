using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using TryggLarm.Admin;
using TryggLarm.NodeEditors;

namespace TryggLarm.Nodes
{
    [Serializable]
    class AlarmObjectNode : CustomNode
    {
        public AlarmObjectNode(string name) : base(name)
        {
        }

        public AlarmObjectNode(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override void OnNodeCreated(NodeEditorManager nodeMng)
        {
            nodeMng.NodeTreeView.BeginUpdate();
            this.Nodes.Add(new AlarmGroupNode("Alarm Group")
            {
                NodeMng = this.NodeMng
            });
            this.Nodes.Add(new SMSRecipientGroupNode("SMS Group")
            {
                NodeMng = this.NodeMng
            });
            this.Nodes.Add(new EmailRecipientGroupNode("Email Group")
            {
                NodeMng = this.NodeMng
            });

            this.Nodes.Add(new NokasGroupNode("Nokas")
            {
                NodeMng = this.NodeMng
            });

            nodeMng.NodeTreeView.EndUpdate();
            this.Expand();

            base.OnNodeCreated(nodeMng);
        }

        public override void Initialize()
        {
            ContextMenu = new System.Windows.Forms.ContextMenu(new System.Windows.Forms.MenuItem[]
            {
                new System.Windows.Forms.MenuItem("Remove Object", RemoveNodeClick)
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

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        public override void InitializeEditor()
        {
            myNodeEditor = new NameNodeEditor(this.NodeMng);
            base.InitializeEditor();
        }
    }
}
