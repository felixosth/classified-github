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
    class EmailRecipientNode : CustomNode
    {
        public string EmailAddress { get; set; }
        public string BodyFormatting { get; set; }
        public string SubjectFormatting { get; set; }
        public decimal AlarmTimeOffset { get; set; }

        public DateTime LastActiveDate { get; set; }
        public int CooldownTime { get; set; }

        public bool AttachCameraImage { get; set; }

        public EmailRecipientNode(string name) : base(name)
        {
            EmailAddress = "";
            BodyFormatting = "";
            SubjectFormatting = "";
            AttachCameraImage = false;
            AlarmTimeOffset = 0;
            CooldownTime = 1;
            LastActiveDate = DateTime.MinValue;
        }

        public EmailRecipientNode(SerializationInfo info, StreamingContext context):base(info,context)
        {
            EmailAddress = (string)info.GetValue("Email", typeof(string));
            BodyFormatting = (string)info.GetValue("Formatting", typeof(string));
            SubjectFormatting = (string)info.GetValue("SubjectFormatting", typeof(string));
            AttachCameraImage = info.GetBoolean("AttachCamImg");
            AlarmTimeOffset = info.GetDecimal("AlarmTimeOffset");
            LastActiveDate = info.GetDateTime("LastActiveDate") ;
            CooldownTime = info.GetInt32("Cooldown");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Email", EmailAddress);
            info.AddValue("Formatting", BodyFormatting);
            info.AddValue("SubjectFormatting", SubjectFormatting);
            info.AddValue("AttachCamImg", AttachCameraImage);
            info.AddValue("AlarmTimeOffset", AlarmTimeOffset);
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
            myNodeEditor = new NodeEditors.EmailRecipientNodeEditor(NodeMng);
            base.InitializeEditor();
        }
    }
}
