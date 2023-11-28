using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using VideoOS.Platform;
using VideoOS.Platform.Messaging;
using VideoOS.Platform.UI;

namespace InSupport.Client
{
    public partial class InSupportControlPanel : Form
    {
        private MessageCommunication _mc;


        enum UsingFunction
        {
            Focus,
            Restart
        }

        UsingFunction executeFunction;

        public InSupportControlPanel()
        {
            InitializeComponent();
            ClientControl.Instance.RegisterUIControlForAutoTheming(this);
            MessageCommunicationManager.Start(EnvironmentManager.Instance.MasterSite.ServerId);
            _mc = MessageCommunicationManager.Get(EnvironmentManager.Instance.MasterSite.ServerId);

            _mc.RegisterCommunicationFilter(IPAddressResponseHandler, new CommunicationIdFilter(MessageId.Server.GetIPAddressResponse));

            ClientControl.Instance.RegisterUIControlForAutoTheming(this);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //using (ItemPickerForm itemPick = new ItemPickerForm())
            //{
            //    itemPick.StartPosition = FormStartPosition.CenterParent;
            //    itemPick.KindFilter = Kind.Camera;
            //    itemPick.ShowDisabledItems = true;
            //    itemPick.Init(Configuration.Instance.GetItems());
            //    if(itemPick.ShowDialog() == DialogResult.OK)
            //    {
            //        executeFunction = UsingFunction.Restart;
            //        _mc.TransmitMessage(new VideoOS.Platform.Messaging.Message(VideoOS.Platform.Messaging.MessageId.Server.GetIPAddressRequest, itemPick.SelectedItem.FQID), null, null, null);
            //    }
            //}

            executeFunction = UsingFunction.Restart;
            OpenCameraDialog();

            CloseAndDispose();

        }


        bool isOpened = false;
        void OpenCameraDialog()
        {
            using (ItemPickerForm itemPicker = new ItemPickerForm())
            {
                itemPicker.StartPosition = FormStartPosition.CenterParent;
                itemPicker.KindFilter = Kind.Camera;
                itemPicker.ShowDisabledItems = true;
                itemPicker.Init(Configuration.Instance.GetItems());
                if (itemPicker.ShowDialog() == DialogResult.OK)
                {
                    isOpened = false;
                    _mc.TransmitMessage(new VideoOS.Platform.Messaging.Message(VideoOS.Platform.Messaging.MessageId.Server.GetIPAddressRequest, itemPicker.SelectedItem.FQID), null, null, null);
                }
            }
        }

        private object IPAddressResponseHandler(VideoOS.Platform.Messaging.Message message, FQID destination, FQID sender)
        {
            if (isOpened)
                return null;
            else
                isOpened = true;

            string ip = (string)message.Data;
            ip = ip.Replace("http://", "").Replace("/", "");

           if(executeFunction == UsingFunction.Focus)
            {
                using (var focusForm = new AxisCameraFocusForm(ip))
                {
                    if (focusForm.ShowDialog() == DialogResult.OK)
                    {

                    }
                }
            }
           else if(executeFunction == UsingFunction.Restart)
            {
                using (var enterCreds = new AxisCamRestartForm(ip))
                {
                    if (enterCreds.ShowDialog() == DialogResult.OK)
                    {

                    }
                }
            }

            //switch (executeFunction)
            //{
            //    case UsingFunction.Focus:
            //        using (var focusForm = new AxisCameraFocusForm(ip))
            //        {
            //            if (focusForm.ShowDialog() == DialogResult.OK)
            //            {

            //            }
            //        }
            //        break;

            //    case UsingFunction.Restart:
            //        using (var enterCreds = new AxisCamRestartForm(ip))
            //        {
            //            if (enterCreds.ShowDialog() == DialogResult.OK)
            //            {

            //            }
            //        }
            //        break;
            //}

            return null;
        }

        private void InSupportControlPanel_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //using (ItemPickerForm itemPick = new ItemPickerForm())
            //{
            //    itemPick.StartPosition = FormStartPosition.CenterParent;
            //    itemPick.KindFilter = Kind.Camera;
            //    itemPick.ShowDisabledItems = true;
            //    itemPick.Init(Configuration.Instance.GetItems());
            //    if (itemPick.ShowDialog() == DialogResult.OK)
            //    {
            //        executeFunction = UsingFunction.Focus;

            //        _mc.TransmitMessage(new VideoOS.Platform.Messaging.Message(VideoOS.Platform.Messaging.MessageId.Server.GetIPAddressRequest, itemPick.SelectedItem.FQID), null, null, null);
            //    }
            //}

            executeFunction = UsingFunction.Focus;
            OpenCameraDialog();

            CloseAndDispose();

        }


        void CloseAndDispose()
        {
            Close();
            Dispose();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var stats = new ShowCustomerCounterStatsForm();
            if (!stats.IsDisposed)
                stats.Show();
            CloseAndDispose();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            new StatusViewer().Show();
            CloseAndDispose();
        }
    }
}
