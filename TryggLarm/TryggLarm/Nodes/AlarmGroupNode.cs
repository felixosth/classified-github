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
    class AlarmGroupNode : CustomNode
    {
        public AlarmGroupNode(string name) : base(name)
        {
        }

        public AlarmGroupNode(SerializationInfo info, StreamingContext context):base(info,context)
        {
        }

        public override void Initialize()
        {
            ContextMenu = new System.Windows.Forms.ContextMenu(new System.Windows.Forms.MenuItem[]
                {
                    new System.Windows.Forms.MenuItem("Add related alarm", AddRelatedAlarmClick)
                });
            base.Initialize();
        }

        void AddRelatedAlarmClick(object s, EventArgs e)
        {
            var newNode = new AlarmNode("Alarm")
            {
                NodeMng = this.NodeMng
            };
            NodeMng.NodeTreeView.BeginUpdate();
            this.Nodes.Add(newNode);
            NodeMng.NodeTreeView.EndUpdate();
            this.Expand();
            NodeMng.NodeTreeView.SelectedNode = newNode;

            NodeMng.OnUserChange(s, e);
        }

        public override void InitializeEditor()
        {
            base.InitializeEditor();
        }
    }
}
