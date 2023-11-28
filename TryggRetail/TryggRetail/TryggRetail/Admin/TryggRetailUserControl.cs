using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using TryggRetail.Actions;
using VideoOS.Platform;
using VideoOS.Platform.Admin;
using VideoOS.Platform.UI;

namespace TryggRetail.Admin
{
    /// <summary>
    /// This UserControl only contains a configuration of the Name for the Item.
    /// The methods and properties are used by the ItemManager, and can be changed as you see fit.
    /// </summary>
    public partial class TryggRetailUserControl : UserControl
    {
        public event EventHandler ConfigurationChangedByUser;
        PopupConfig myConfig;

        public TryggRetailUserControl()
        {
            InitializeComponent();

            alarmSignalTxtBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            alarmSignalTxtBox.AutoCompleteCustomSource = new AutoCompleteStringCollection()
            {
                "alarm 1.wav",
                "alarm 2.wav",
                "alarm 3.wav"
            };
            alarmSignalTxtBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        }

        internal String DisplayName
        {
            get { return textBoxName.Text; }
            set { textBoxName.Text = value; }
        }

        /// <summary>
        /// Ensure that all user entries will call this method to enable the Save button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void OnUserChange(object sender, EventArgs e)
        {
            if (myConfig != null && !isFilling)
            {
                myConfig.AckTime = (int)ackTimeNum.Value;
                myConfig.AlarmSignal = alarmSignalTxtBox.Text;
                myConfig.WindowPosition.Y = (int)posYNum.Value;
                myConfig.WindowPosition.X = (int)posXNum.Value;
                myConfig.WindowSize.Width = (int)widthNum.Value;
                myConfig.WindowSize.Height = (int)heightNum.Value;
                myConfig.AlarmName = alarmNameTxtBox.Text;
                myConfig.AlarmPlaybackOffsetBefore = (int)playbackOffsetBeforeNum.Value;
                myConfig.IsFullscreen = fullscreenChkBox.Checked;
                myConfig.LoopByDefault = loopDefChkBox.Checked;
                myConfig.AlarmPlaybackOffsetAfter = (int)playbackOffsetAfterNum.Value;
                myConfig.LoopAlarmSignal = loopSoundChkBox.Checked;
                myConfig.StopSoundOnExit = stopSoundOnExitChkBox.Checked;
                myConfig.ButtonBlink = buttonBlinkChkBox.Checked;
                myConfig.CustomActions.Clear();

                foreach(CustomAction actionItem in customActionsBox.Items)
                {
                    myConfig.CustomActions.Add(actionItem);
                }
            }

            ConfigurationChangedByUser?.Invoke(this, new EventArgs());
        }


        bool isFilling = true;
        public void FillContent(Item item)
        {
            isFilling = true;

            textBoxName.Text = item.Name;

            if(item.Properties.ContainsKey("configuration"))
            {
                myConfig = PopupConfig.Deserialize(item.Properties["configuration"]);

                alarmNameTxtBox.Text = myConfig.AlarmName;
                ackTimeNum.Value = (decimal)myConfig.AckTime;
                playbackOffsetBeforeNum.Value = (decimal)myConfig.AlarmPlaybackOffsetBefore;
                posXNum.Value = (decimal)myConfig.WindowPosition.X;
                posYNum.Value = (decimal)myConfig.WindowPosition.Y;
                buttonBlinkChkBox.Checked = myConfig.ButtonBlink;

                stopSoundOnExitChkBox.Checked = myConfig.StopSoundOnExit;

                widthNum.Value = (decimal)myConfig.WindowSize.Width;
                heightNum.Value = (decimal)myConfig.WindowSize.Height;
                alarmSignalTxtBox.Text = myConfig.AlarmSignal;
                fullscreenChkBox.Checked = myConfig.IsFullscreen;
                loopDefChkBox.Checked = myConfig.LoopByDefault;
                if (myConfig.CustomActions == null)
                    myConfig.CustomActions = new System.Collections.Generic.List<CustomAction>();
                loopSoundChkBox.Checked = myConfig.LoopAlarmSignal;
                //maxWindowsNum.Value = (decimal)myConfig.MaxWindows;
                foreach(CustomAction action in myConfig.CustomActions)
                {
                    customActionsBox.Items.Add(action);
                }
            }
            else
            {
                myConfig = new PopupConfig();
                //myConfig.CustomActions = new System.Collections.Generic.List<CustomAction>();
            }

            isFilling = false;
        }

        public void UpdateItem(Item item)
        {
            item.Name = DisplayName;

            item.Properties["configuration"] = myConfig.Serialize();
        }

        public void ClearContent()
        {
            myConfig = null;
            textBoxName.Text = "";
            alarmNameTxtBox.Text = "";
            ackTimeNum.Value = 3;
            playbackOffsetBeforeNum.Value = 3;
            fullscreenChkBox.Checked = false;
            posXNum.Value = 0;
            posYNum.Value = 0;
            loopDefChkBox.Checked = false;
            widthNum.Value = 1280;
            heightNum.Value = 720;
            loopSoundChkBox.Checked = false;
            alarmSignalTxtBox.Text = "";
            playbackOffsetAfterNum.Value = 3;
            customActionsBox.Items.Clear();

            buttonBlinkChkBox.Checked = false;
            stopSoundOnExitChkBox.Checked = false;

            foreach (Control control in windowPropGrp.Controls)
            {
                if (!(control is CheckBox))
                {
                    control.Enabled = true;
                }
            }
        }

        private void fullscreenChkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (myConfig == null)
                return;
            myConfig.IsFullscreen = fullscreenChkBox.Checked;
            OnUserChange(sender, e);
            foreach (Control control in windowPropGrp.Controls)
            {
                if (!(control is CheckBox))
                {
                    control.Enabled = !fullscreenChkBox.Checked;
                }
            }
        }

        private void addActionBtn_Click(object sender, EventArgs e)
        {
            using (var actionEditor = new ActionEditorForm())
            {
                if(actionEditor.ShowDialog() == DialogResult.OK)
                {
                    customActionsBox.Items.Add(actionEditor.CustomAction);
                    OnUserChange(sender, e);
                }
            }
        }

        private void removeActionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            customActionsBox.Items.Remove(customActionsBox.SelectedItem);
            OnUserChange(sender, e);
        }

        private void customActionsContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            removeActionToolStripMenuItem.Enabled = customActionsBox.SelectedItem != null;
        }

        private void customActionsBox_MouseDown(object sender, MouseEventArgs e)
        {
            customActionsBox.SelectedIndex = customActionsBox.IndexFromPoint(e.X, e.Y);
        }
    }
}
