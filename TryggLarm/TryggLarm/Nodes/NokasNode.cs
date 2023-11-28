using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TryggLarm.Nodes
{
    [Serializable]
    class NokasNode : CustomNode
    {
        public string Code { get; set; }
        public string Pin { get; set; }
        public string Type { get; set; }

        public string Info { get; set; }

        public NokasNode(string name) : base(name)
        {
            Code = "";
            Pin = "";
            Type = "";
        }


        public NokasNode(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Code = (string)info.GetValue("Code", typeof(string));
            Pin = (string)info.GetValue("Pin", typeof(string));
            Type = (string)info.GetValue("Type", typeof(string));
            Info = (string)info.GetValue("Info", typeof(string));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Code", Code);
            info.AddValue("Pin", Pin);
            info.AddValue("Type", Type);
            info.AddValue("Info", Info);

            base.GetObjectData(info, context);
        }

        public override void Initialize()
        {
            ContextMenu = new System.Windows.Forms.ContextMenu(new System.Windows.Forms.MenuItem[]
            {
                new System.Windows.Forms.MenuItem(IsEnabled ? "Disable" : "Enable", DisableNodeClick),
                new System.Windows.Forms.MenuItem("Remove", RemoveNodeClick)
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
            myNodeEditor = new NodeEditors.NokasNodeEditor(NodeMng);
            base.InitializeEditor();
        }
    }
}
