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
    public partial class NokasNodeEditor : NodeEditorBase
    {
        public NokasNodeEditor(NodeEditorManager nodeMng) : base(nodeMng)
        {
            InitializeComponent();
        }

        public override void LoadValues(CustomNode node)
        {
            var nokasNode = node as NokasNode;
            if (nokasNode == null)
                throw new Exception("Wrong node editor");

            codeTxtBox.Text = nokasNode.Code;
            pinTxtBox.Text = nokasNode.Pin;
            infoTxtBox.Text = nokasNode.Info;
            typeTxtBox.Text = nokasNode.Type;

            enabledChkBox.Checked = nokasNode.IsEnabled;


            codeTxtBox.TextChanged += ModifiedValues;
            pinTxtBox.TextChanged += ModifiedValues;
            infoTxtBox.TextChanged += ModifiedValues;
            typeTxtBox.TextChanged += ModifiedValues;
            enabledChkBox.CheckedChanged += ModifiedValues;

            base.LoadValues(node);
        }

        public override void SetValues()
        {
            var nokasNode = myNode as NokasNode;

            nokasNode.Code = codeTxtBox.Text;
            nokasNode.Pin = pinTxtBox.Text;
            nokasNode.Info = infoTxtBox.Text;
            nokasNode.Type = typeTxtBox.Text;
            nokasNode.IsEnabled = enabledChkBox.Checked;

            base.SetValues();
        }

        public override void ModifiedValues(object sender, EventArgs e)
        {
            SetValues();
            base.ModifiedValues(sender, e);
        }

        private void infoTxtBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
