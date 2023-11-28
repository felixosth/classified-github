using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TryggLarm.Admin;
using TryggLarm.Nodes;

namespace TryggLarm.NodeEditors
{
    public partial class EmailRecipientNodeEditor : NodeEditorBase
    {
        public EmailRecipientNodeEditor(NodeEditorManager nodeMng):base(nodeMng)
        {
            InitializeComponent();
        }

        public override void LoadValues(CustomNode node)
        {
            var emailNode = node as EmailRecipientNode;
            if (node == null)
                throw new Exception("Wrong node editor");

            nameTxtBox.Text = emailNode.DisplayName;
            emailTxtBox.Text = emailNode.EmailAddress;
            formattingTxtBox.Text = emailNode.BodyFormatting;
            subjectTxtBox.Text = emailNode.SubjectFormatting;
            attachCamImgChkBox.Checked = emailNode.AttachCameraImage;
            alarmOffsetNum.Value = emailNode.AlarmTimeOffset;
            cooldownNum.Value = emailNode.CooldownTime;
            enabledChkBox.Checked = emailNode.IsEnabled;

            foreach (object control in Controls)
            {
                if (control is TextBox)
                    (control as TextBox).TextChanged += ModifiedValues;
            }
            foreach(object control in groupBox1.Controls)
            {
                if (control is TextBox)
                    (control as TextBox).TextChanged += ModifiedValues;
            }

            enabledChkBox.CheckedChanged += ModifiedValues;

            attachCamImgChkBox.CheckedChanged += ModifiedValues;
            alarmOffsetNum.ValueChanged += ModifiedValues;
            cooldownNum.ValueChanged += ModifiedValues;

            base.LoadValues(node);
        }

        public override void SetValues()
        {
            var emailNode = myNode as EmailRecipientNode;
            emailNode.DisplayName = nameTxtBox.Text;
            emailNode.EmailAddress = emailTxtBox.Text;
            emailNode.BodyFormatting = formattingTxtBox.Text;
            emailNode.SubjectFormatting = subjectTxtBox.Text;
            emailNode.AttachCameraImage = attachCamImgChkBox.Checked;
            emailNode.AlarmTimeOffset = alarmOffsetNum.Value;
            emailNode.CooldownTime = (int)cooldownNum.Value;
            emailNode.IsEnabled = enabledChkBox.Checked;

            base.SetValues();
        }

        public override void ModifiedValues(object sender, EventArgs e)
        {
            SetValues();
            base.ModifiedValues(sender, e);
        }

        private void EmailRecipientNodeEditor_Load(object sender, EventArgs e)
        {

        }
    }
}
