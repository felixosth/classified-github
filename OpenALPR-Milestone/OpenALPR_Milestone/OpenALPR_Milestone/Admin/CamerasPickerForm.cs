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

namespace OpenALPR_Milestone.Admin
{
    public partial class CamerasPickerForm : Form
    {
        ItemPickerUserControl itemPickerUserControl;

        public CamerasPickerForm(List<Item> existingCameras = null)
        {
            InitializeComponent();

            itemPickerUserControl = new ItemPickerUserControl()
            {
                ItemsToSelectFrom = Configuration.Instance.GetItemsByKind(Kind.Camera),
                CategoryUserSelectable = false,
                ItemsSelected = existingCameras,
                //KindUserSelectable = true,
                //KindSelected = new Guid("{f3b0a519-ce2f-4292-b637-97f93d441859}"),
                ServerTabVisible = true,
                GroupTabVisible = true,
                //KindSelected = Kind.Camera,
                KindUserSelectable = false,
                //KindFilter = new List<Guid>() { Kind.Camera },
                Dock = DockStyle.Fill,
            };
            itemPickerUserControl.Init();

            itemPickerContainer.Controls.Add(itemPickerUserControl);
        }

        public List<Item> SelectedItems => itemPickerUserControl.ItemsSelected;

        private void OkBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void CamerasPickerForm_Load(object sender, EventArgs e)
        {

        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
