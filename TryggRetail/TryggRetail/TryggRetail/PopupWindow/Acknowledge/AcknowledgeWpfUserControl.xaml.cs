using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TryggRetail.Admin;
using VideoOS.Platform.Client;
using VideoOS.Platform.Messaging;
using VideoOS.Platform;

namespace TryggRetail.PopupWindow.Acknowledge
{
    /// <summary>
    /// Interaction logic for AcknowledgeWpfUserControl.xaml
    /// </summary>
    public partial class AcknowledgeWpfUserControl : ViewItemWpfUserControl
    {
        bool isAck = false, completedAck = false;

        PopupConfig popupConfig;
        Background.TryggRetailBackgroundPlugin instance;

        FQID myWindow;

        public AcknowledgeWpfUserControl(/*PopupConfig popConfig, */Background.TryggRetailBackgroundPlugin instance)
        {
            this.instance = instance;
            popupConfig = instance.CurrentPopupConfig;
            InitializeComponent();

            ackBtn.Content += "\r\n" + popupConfig.AlarmName;


            new Thread(() =>
            {
                myWindow = instance.GetDataWindowFQID();
                instance.NullifyWindowData();

            }).Start();
        }

        private void AcknowledgeCounter()
        {
            for (int i = (int)popupConfig.AckTime; i > 0; i--)

            {
                //ackBtn.Content = "Stäng Fönster (" + i + ")";

                Dispatcher.Invoke(new Action(() =>
                {
                    ackBtn.Content = "Stäng Fönster (" + i + ")";
                }));
                //if (!Dispatcher.CheckAccess())
                //{

                //}
                Thread.Sleep(1000);
            }
            //if (!Dispatcher.CheckAccess())
            //{
            Dispatcher.Invoke(new Action(() =>
            {
                ackBtn.Background = Brushes.Green;
                ackBtn.Content = "Stäng Fönster";
                //ackBtn.IsEnabled = true;
                

            }));
            //}
            completedAck = true;
        }

        private void AckButton_Click(object sender, RoutedEventArgs e)
        {
            if (isAck && completedAck)
            {

                Dispatcher.Invoke(new Action(() =>
                {
                    List<Item> list = Configuration.Instance.GetItemsByKind(Kind.Window);
                    MultiWindowCommandData data = new MultiWindowCommandData();

                    data.Screen = null;
                    data.View = null;
                    data.Window = myWindow;
                    data.X = 200;
                    data.Y = 200;
                    data.Height = 400;
                    data.Width = 400;
                    data.PlaybackSupportedInFloatingWindow = true;

                    data.MultiWindowCommand = MultiWindowCommand.CloseSelectedWindow;
                    EnvironmentManager.Instance.SendMessage(new VideoOS.Platform.Messaging.Message(MessageId.SmartClient.MultiWindowCommand, data), null, null);
                }));

            }
            else if (!isAck)
            {
                isAck = true;
                //ackBtn.IsEnabled = false;
                ackBtn.Background = Brushes.Yellow;
                ackBtn.Foreground = Brushes.Black;
                new Thread(AcknowledgeCounter).Start();
            }
        }

        public override bool ShowToolbar => false;
    }
}
