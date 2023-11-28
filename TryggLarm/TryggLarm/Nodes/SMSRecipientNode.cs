using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TryggLarm.Nodes
{
    [Serializable]
    class SMSRecipientNode : CustomNode
    {
        public string TelephoneNumber { get; set; }
        public string Formatting { get; set; }

        public DateTime LastActiveDate { get; set; }
        public int CooldownTime { get; set; }

        public SMSRecipientNode(string name) : base(name)
        {
            TelephoneNumber = "";
            Formatting = "";
            CooldownTime = 1;
            LastActiveDate = DateTime.MinValue;
        }

        public SMSRecipientNode(SerializationInfo info, StreamingContext context):base(info,context)
        {
            TelephoneNumber = (string)info.GetValue("Tel", typeof(string));
            Formatting = (string)info.GetValue("Formatting", typeof(string));
            LastActiveDate = info.GetDateTime("LastActiveDate");
            CooldownTime = info.GetInt32("Cooldown");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Tel", TelephoneNumber);
            info.AddValue("Formatting", Formatting);
            info.AddValue("LastActiveDate", LastActiveDate);
            info.AddValue("Cooldown", CooldownTime);
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

        internal override void DisableNodeClick(object sender, EventArgs e)
        {
            base.DisableNodeClick(sender, e);
            RefreshMyEditor();
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
            myNodeEditor = new NodeEditors.SMSRecipientNodeEditor(NodeMng);
            base.InitializeEditor();
        }

    }
}
