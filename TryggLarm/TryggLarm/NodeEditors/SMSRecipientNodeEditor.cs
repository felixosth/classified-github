using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TryggLarm.Nodes;
using TryggLarm.Admin;

namespace TryggLarm.NodeEditors
{
    public partial class SMSRecipientNodeEditor : NodeEditorBase
    {
        public SMSRecipientNodeEditor(NodeEditorManager nodeMng) : base(nodeMng)
        {
            InitializeComponent();
        }

        private void SMSRecipientNodeEditor_Load(object sender, EventArgs e)
        {

        }

        public override void LoadValues(CustomNode node)
        {
            var smsNode = node as SMSRecipientNode;
            if (node == null)
                throw new Exception("Wrong node editor");
            nameTxtBox.Text = smsNode.DisplayName;
            telTxtBox.Text = smsNode.TelephoneNumber;
            formattingTxtBox.Text = smsNode.Formatting;
            cooldownNum.Value = smsNode.CooldownTime;
            enabledChkBox.Checked = smsNode.IsEnabled;

            //foreach (object control in Controls)
            //{
            //    if(control is TextBox)
            //        (control as TextBox).TextChanged += ModifiedValues;
            //}
            nameTxtBox.TextChanged += ModifiedValues;
            formattingTxtBox.TextChanged += ModifiedValues;
            telTxtBox.TextChanged += ModifiedValues;
            cooldownNum.ValueChanged += ModifiedValues;
            enabledChkBox.CheckedChanged += ModifiedValues;
            //telTxtBox.TextChanged += ModifiedValues;

            base.LoadValues(node);
        }

        public override void SetValues()
        {
            var smsNode = myNode as SMSRecipientNode;
            smsNode.DisplayName = nameTxtBox.Text;
            smsNode.TelephoneNumber = telTxtBox.Text;
            smsNode.Formatting = formattingTxtBox.Text;
            smsNode.CooldownTime = (int)cooldownNum.Value;
            smsNode.IsEnabled = enabledChkBox.Checked;

            base.SetValues();
        }

        public override void ModifiedValues(object sender, EventArgs e)
        {
            SetValues();
            base.ModifiedValues(sender, e);
        }
    }
}
