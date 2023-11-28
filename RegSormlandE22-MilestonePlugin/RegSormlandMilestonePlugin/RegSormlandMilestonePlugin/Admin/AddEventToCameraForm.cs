using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VideoOS.Platform;
using VideoOS.Platform.UI;

namespace RegSormlandMilestonePlugin.Admin
{
    public partial class AddEventToCameraForm : Form
    {
        public Item SelectedEvent { get; set; }
        public Item SelectedBtnEvent { get; set; }
        public int TimeToDisplay => (int)timeToDisplayNumVal.Value;
        public string ButtonText
        {
            get { return buttonTextTxtBox.Text; }
            set { buttonTextTxtBox.Text = value; }
        }

        public AddEventToCameraForm()
        {
            InitializeComponent();
        }

        private void browseEventBtn_Click(object sender, EventArgs e)
        {
            var selectedEvent = PickEvent();
            if(selectedEvent != null)
            {
                SelectedEvent = selectedEvent;
                eventTxtBox.Text = SelectedEvent.Name;
            }
        }

        private void browseBtnEventBtn_Click(object sender, EventArgs e)
        {
            var selectedEvent = PickEvent();
            if (selectedEvent != null)
            {
                SelectedBtnEvent = selectedEvent;
                btnEventTxtBox.Text = SelectedBtnEvent.Name;
            }
        }

        private Item PickEvent()
        {
            using (var itemPicker = new ItemPickerForm())
            {
                itemPicker.Init(Configuration.Instance.GetItemsByKind(Kind.TriggerEvent, ItemHierarchy.SystemDefined));
                itemPicker.KindFilter = Kind.TriggerEvent;
                if (itemPicker.ShowDialog() == DialogResult.OK)
                {
                    return itemPicker.SelectedItem;
                    //selectedItem.SubItems[1].Text = itemPicker.SelectedItem.Name;
                    //config.CamerasEventsDictionary[selectedCam.FQID.SerializeXml()] = itemPicker.SelectedItem.FQID.SerializeXml();
                    //OnUserChange(sender, new EventArgs());
                }
            }
            return null;
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }


    }
}
