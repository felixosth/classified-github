using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TryggRetail.Actions
{
    public partial class ActionEditorForm : Form
    {
        //ActionTypes actionTypes = ActionTypes.HttpRequest;
        CustomActionEditor_UsrControl curEditor;

        public ActionEditorForm()
        {
            InitializeComponent();
            actionTypeComboBox.SelectedIndex = 0;
        }

        private void ActionEditorForm_Load(object sender, EventArgs e)
        {

        }

        private void actionTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            customActionEditorContainer.Controls.Clear();
            switch(actionTypeComboBox.SelectedIndex)
            {
                case (int)ActionTypes.HttpRequest:
                    curEditor = new PlayVapixClip_Editor();
                    break;

                case (int)ActionTypes.TriggerEvent:
                    curEditor = new TriggerEvent_Editor();
                    break;
            }
            customActionEditorContainer.Controls.Add(curEditor);
        }


        private CustomAction customAction;

        /// <summary>
        /// Get created CustomAction
        /// </summary>
        public CustomAction CustomAction
        {
            get { return customAction; }
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            if(displayTextBox.Text == "")
            {
                MessageBox.Show("Missing Display Text", "Error");
                return;
            }
            if (!curEditor.InputIsValid())
            {
                MessageBox.Show("Missing fields", "Error");
                return;
            }

            customAction = curEditor.GetCustomAction();
            customAction.DisplayText = displayTextBox.Text;
            customAction.ButtonBackColor = btnBackColDisplay.BackColor;
            customAction.ButtonForeColor = btnTxtColDisplay.BackColor;

            this.DialogResult = DialogResult.OK;
        }

        private void btnTxtColDisplay_Click(object sender, EventArgs e)
        {
            if(colorDialog1.ShowDialog() == DialogResult.OK)
            {
                btnTxtColDisplay.BackColor = colorDialog1.Color;
            }
        }

        private void btnBackColDisplay_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                btnBackColDisplay.BackColor = colorDialog1.Color;
            }
        }
    }

    enum ActionTypes
    {
        HttpRequest = 0,
        TriggerEvent = 1
    }
}
