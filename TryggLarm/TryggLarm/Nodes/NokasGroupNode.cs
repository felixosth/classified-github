using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TryggLarm.Nodes
{
    [Serializable]
    class NokasGroupNode : CustomNode
    {
        public NokasGroupNode(string name) : base(name)
        {
        }
        public NokasGroupNode(SerializationInfo info, StreamingContext context) : base(info, context) { }


        public override void Initialize()
        {
            ContextMenu = new System.Windows.Forms.ContextMenu(new System.Windows.Forms.MenuItem[]
            {
                new System.Windows.Forms.MenuItem("Add Nokas node", AddNewNokasNode)
            });

            ContextMenu.Popup += (s, e) =>
            {
                ContextMenu.MenuItems[0].Enabled = true;
                foreach(var node in this.Nodes)
                {
                    if (node is NokasNode)
                    {
                        ContextMenu.MenuItems[0].Enabled = false;
                        break;
                    }
                }
            };

            base.Initialize();
        }

        void AddNewNokasNode(object sender, EventArgs e)
        {
            var newNode = new NokasNode("Nokas Alarm")
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
