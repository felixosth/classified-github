using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VideoOS.Platform;
using VideoOS.Platform.UI;

namespace TryggRetail.Actions
{
    public partial class TriggerEvent_Editor : CustomActionEditor_UsrControl
    {
        Item pickedEvent;
        
        public TriggerEvent_Editor()
        {
            InitializeComponent();
        }

        public override CustomAction GetCustomAction()
        {
            return new TriggerEvent_Action()
            {
                SerializedEvent = pickedEvent.Serialize()
            };
            //return base.GetCustomAction(displayText);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (ItemPickerForm itemPicker = new ItemPickerForm())
            {
                itemPicker.KindFilter = Kind.TriggerEvent;

                var events = Configuration.Instance.GetItemsByKind(Kind.TriggerEvent)[0].GetChildren()[0].GetChildren();
                itemPicker.Init(events, ItemHierarchy.SystemDefined);

                if (itemPicker.ShowDialog() == DialogResult.OK)
                {
                    pickedEvent = itemPicker.SelectedItem;
                    selectedEventLabel.Text = pickedEvent.Name;
                }
            }
        }

        public override bool InputIsValid()
        {
            return pickedEvent != null;
        }
    }
}
