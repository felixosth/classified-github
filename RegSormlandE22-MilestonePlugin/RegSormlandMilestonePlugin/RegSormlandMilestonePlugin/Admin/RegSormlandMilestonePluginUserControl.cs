using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using VideoOS.Platform;
using VideoOS.Platform.Admin;
using VideoOS.Platform.UI;
using RegSormlandMilestonePlugin.Shared;

namespace RegSormlandMilestonePlugin.Admin
{
    /// <summary>
    /// This UserControl only contains a configuration of the Name for the Item.
    /// The methods and properties are used by the ItemManager, and can be changed as you see fit.
    /// </summary>
    public partial class RegSormlandMilestonePluginUserControl : UserControl
    {
        internal event EventHandler ConfigurationChangedByUser;
        private RegSormlandConfig config;

        public RegSormlandMilestonePluginUserControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Ensure that all user entries will call this method to enable the Save button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void OnUserChange(object sender, EventArgs e)
        {
            if (ConfigurationChangedByUser != null)
                ConfigurationChangedByUser(this, new EventArgs());
        }

        internal void FillContent(Item item)
        {
            camerasListView.Items.Clear();

            if (item.Properties.ContainsKey(RegSormlandConfig._CFG_KEY))
            {
                try
                {
                    config = RegSormlandConfig.Deserialize(item.Properties[RegSormlandConfig._CFG_KEY]);
                }
                catch
                {
                    MessageBox.Show("Unable to load config, it might be corrupted or outdated.");
                    config = new RegSormlandConfig();
                }
            }
            else
            {
                config = new RegSormlandConfig();
            }

            var cameras = Helper.GetAllItems(Configuration.Instance.GetItemsByKind(Kind.Camera));

            foreach(var cam in cameras)
            {
                var newListViewItem = new ListViewItem(cam.Name);
                newListViewItem.Tag = cam;

                var serializedFqid = cam.FQID.SerializeXml();
                
                if (config.CamerasEventsDictionary.ContainsKey(serializedFqid) && config.CamerasEventsDictionary[serializedFqid] != null)
                {
                    //var eventFqid = new FQID(config.CamerasEventsDictionary[serializedFqid].EventFQIDXml);
                    var camerasEvent = Configuration.Instance.GetItem(config.CamerasEventsDictionary[serializedFqid].EventToFQID());
                    var buttonEvent = Configuration.Instance.GetItem(config.CamerasEventsDictionary[serializedFqid].ButtonEventToFQID());
                    newListViewItem.SubItems.Add(camerasEvent?.Name ?? "Not available");
                    newListViewItem.SubItems.Add(buttonEvent?.Name ?? "Not available");
                    newListViewItem.SubItems.Add(config.CamerasEventsDictionary[serializedFqid].TimeToDisplay.ToString());
                }
                else
                {
                    newListViewItem.SubItems.Add("Not available");
                    newListViewItem.SubItems.Add("Not available");
                    newListViewItem.SubItems.Add("30");
                }
                camerasListView.Items.Add(newListViewItem);
            }

            camerasListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        internal void UpdateItem(Item item)
        {
            item.Properties[RegSormlandConfig._CFG_KEY] = config.Serialize();
        }

        internal void ClearContent()
        {
            camerasListView.Items.Clear();
            config = null;
        }

        private void camerasListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if(camerasListView.SelectedItems.Count > 0)
            {
                var selectedItem = camerasListView.SelectedItems[0];
                var selectedCam = selectedItem.Tag as Item;

                var camEvent = config.CamerasEventsDictionary.ContainsKey(selectedCam.FQID.SerializeXml()) ? config.CamerasEventsDictionary[selectedCam.FQID.SerializeXml()] : null;

                using (var addEvtToCameraForm = new AddEventToCameraForm())
                {
                    addEvtToCameraForm.cameraTxtBox.Text = selectedCam.Name;

                    addEvtToCameraForm.eventTxtBox.Text = selectedItem.SubItems[1].Text;
                    addEvtToCameraForm.btnEventTxtBox.Text = selectedItem.SubItems[2].Text;
                    addEvtToCameraForm.timeToDisplayNumVal.Value = camEvent?.TimeToDisplay ?? 30;

                    if(camEvent != null)
                    {
                        var selectedEventFqid = camEvent.EventToFQID();
                        if(selectedEventFqid != null)
                        {
                            var selectedEvent = Configuration.Instance.GetItem(selectedEventFqid);
                            addEvtToCameraForm.SelectedEvent = selectedEvent;
                        }

                        var selectedBtnEventFqid = camEvent.ButtonEventToFQID();
                        if (selectedBtnEventFqid != null)
                        {
                            var selectedBtnEvent = Configuration.Instance.GetItem(selectedBtnEventFqid);
                            addEvtToCameraForm.SelectedBtnEvent = selectedBtnEvent;
                        }

                        addEvtToCameraForm.ButtonText = camEvent.ButtonEventDisplayText ?? "";
                    }

                    if(addEvtToCameraForm.ShowDialog() == DialogResult.OK)
                    {
                        if(addEvtToCameraForm.SelectedEvent != null)
                            selectedItem.SubItems[1].Text = addEvtToCameraForm.SelectedEvent.Name;
                        
                        if (addEvtToCameraForm.SelectedBtnEvent != null)
                            selectedItem.SubItems[2].Text = addEvtToCameraForm.SelectedBtnEvent.Name;

                        selectedItem.SubItems[3].Text = addEvtToCameraForm.TimeToDisplay.ToString();

                        config.CamerasEventsDictionary[selectedCam.FQID.SerializeXml()] = new CameraEvent() 
                        { 
                            EventFQIDXml = addEvtToCameraForm.SelectedEvent?.FQID.SerializeXml(), 
                            ButtonEventFQIDXml = addEvtToCameraForm.SelectedBtnEvent?.FQID.SerializeXml(),
                            TimeToDisplay =  addEvtToCameraForm.TimeToDisplay,
                            ButtonEventDisplayText = addEvtToCameraForm.ButtonText
                        };

                        OnUserChange(sender, new EventArgs());
                    }
                }
            }
        }

        private void camerasListViewContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = camerasListView.SelectedItems.Count == 0;
        }

        private void removeEventToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectedItem = camerasListView.SelectedItems[0];
            var selectedCam = selectedItem.Tag as Item;

            config.CamerasEventsDictionary.Remove(selectedCam.FQID.SerializeXml());

            selectedItem.SubItems[1].Text = "Not available";
            selectedItem.SubItems[2].Text = "Not available";
            selectedItem.SubItems[3].Text = "30";

            OnUserChange(sender, e);
        }
    }
}
