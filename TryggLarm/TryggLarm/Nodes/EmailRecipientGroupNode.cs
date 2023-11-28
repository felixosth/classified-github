using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TryggLarm.Nodes
{
    [Serializable]
    class EmailRecipientGroupNode : CustomNode
    {
        public EmailRecipientGroupNode(string name) : base(name) { }

        public EmailRecipientGroupNode(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public override void Initialize()
        {
            ContextMenu = new System.Windows.Forms.ContextMenu(new System.Windows.Forms.MenuItem[]
            {
                new System.Windows.Forms.MenuItem("Add new Email Recipient", AddNewRecipientClick)
            });

            base.Initialize();
        }

        void AddNewRecipientClick(object sender, EventArgs e)
        {
            var newNode = new EmailRecipientNode("Email Recipient")
            {
                NodeMng = this.NodeMng
            };
            NodeMng.NodeTreeView.BeginUpdate();
            this.Nodes.Add(newNode);
            NodeMng.NodeTreeView.EndUpdate();
            this.Expand();

            NodeMng.NodeTreeView.SelectedNode = newNode;
            NodeMng.OnUserChange(sender, e);
        }
    }
}
