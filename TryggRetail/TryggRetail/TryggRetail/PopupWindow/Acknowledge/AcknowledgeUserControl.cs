using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TryggRetail.Background;
using VideoOS.Platform.Client;
using TryggRetail.Admin;
using VideoOS.Platform;
using System.Threading;
using VideoOS.Platform.Messaging;
using TryggRetail.Actions;
using System.Media;
using System.IO;

namespace TryggRetail.PopupWindow.Acknowledge
{
    public partial class AcknowledgeUserControl : ViewItemUserControl
    {
        bool isAck = false, completedAck = false;

        FQID myWindow;

        PopupConfig popupConfig;
        TryggRetailBackgroundPlugin instance;
        Playback.PlaybackAlarmUserControl playbacker;

        SoundPlayer soundPlayer;

        public AcknowledgeUserControl(TryggRetailBackgroundPlugin instance)
        {
            InitializeComponent();

            EnvironmentManager.Instance.RegisterReceiver(new MessageReceiver(CloseAllWindowsReciever), new MessageIdFilter(TryggRetailDefinition.CloseAllWindowsFilter));
            this.instance = instance;
            this.popupConfig = instance.CurrentPopupConfig;
            this.playbacker = instance.CurrentPlaybacker;
            ackBtn.Text += "\r\n" + instance.CurrentAlarm.EventHeader.Name + " (" + instance.CurrentAlarm.EventHeader.Timestamp.ToShortTimeString() + ")";

            if(popupConfig.LoopByDefault)
            {
                loopBtn.BackColor = Color.Gold;
                loopBtn.Text = "Stoppa\r\nLoop";
            }

            new Thread(() =>
            {
                myWindow = instance.GetDataWindowFQID();
                instance.NullifyWindowData();

            }).Start();

            foreach(var action in popupConfig.CustomActions)
            {
                actionButtonsPanel.Controls.Add(new CustomActionBtn(action));
            }
            if (actionButtonsPanel.Controls.Count < 1)
                actionButtonsPanel_Resize(new object(), new EventArgs());

            var soundLoc = @"C:\Program Files\Milestone\MIPPlugins\TryggRetail\Sounds\" + popupConfig.AlarmSignal;

            if(File.Exists(soundLoc))
            {
                soundPlayer = new SoundPlayer(soundLoc);
                if (popupConfig.LoopAlarmSignal)
                    soundPlayer.PlayLooping();
                else
                    soundPlayer.Play();
            }

            this.Dock = DockStyle.Fill;
        }

        private void AcknowledgeUserControl_Load(object sender, EventArgs e)
        {
            if(popupConfig.ButtonBlink)
                new Thread(buttonBlink).Start();
        }

        private void buttonBlink()
        {
            int sleep = 1000 / 4;
            while (!isAck)
            {
                ackBtn.Invoke(new Action(() => ChangeBtnColor(Color.Yellow)));

                Thread.Sleep(sleep);

                ackBtn.Invoke(new Action(() => ChangeBtnColor(Color.Red)));

                Thread.Sleep(sleep);
            }

            if (ackBtn.ForeColor != Color.Yellow)
                ackBtn.Invoke(new Action(() => ChangeBtnColor(Color.Yellow)));
        }

        private void ChangeBtnColor(Color color)
        {
            ackBtn.BackColor = color;
        }

        private object CloseAllWindowsReciever(VideoOS.Platform.Messaging.Message message, FQID dest, FQID source)
        {
            closeWindow();
            return null;
        }

        private void closeWindow()
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
        }

        private void ackBtn_Click(object sender, EventArgs e)
        {
            if (isAck && completedAck)
            {
                closeWindow();
            }
            else if (!isAck)
            {
                isAck = true;
                if(!popupConfig.StopSoundOnExit)
                    soundPlayer?.Stop();
                //ackBtn.IsEnabled = false;
                ackBtn.BackColor = Color.Yellow;
                ackBtn.ForeColor = Color.Black;
                //new AlarmCommandClient().Acknowledge(instance.CurrentAlarm.EventHeader.ID);

                new Thread(AcknowledgeCounter).Start();
            }
        }

        private void AcknowledgeCounter()
        {
            for (int i = (int)popupConfig.AckTime; i > 0; i--)

            {
                this.Invoke(new Action(() =>
                {
                    ackBtn.Text = "Stäng Fönster (" + i + "s)";
                }));
                Thread.Sleep(1000);
            }
            this.Invoke(new Action(() =>
            {
                ackBtn.BackColor = Color.Green;
                ackBtn.ForeColor = Color.White;
                ackBtn.Text = "Stäng Fönster";
            }));
            completedAck = true;
        }

        private void loopBtn_Click(object sender, EventArgs e)
        {
            if (!playbacker.LoopPlayback)
            {
                playbacker.LoopPlayback = true;
                loopBtn.BackColor = Color.Gold;
                loopBtn.Text = "Stoppa\r\nLoop";
            }
            else
            {
                playbacker.LoopPlayback = false;
                loopBtn.Text = "Starta\r\nLoop";
                loopBtn.BackColor = Color.Orange;
            }
        }

        public override void Close()
        {
            instance.WindowCount--;
            soundPlayer?.Stop();
        }

        private void replayBtn_Click(object sender, EventArgs e)
        {
            playbacker.RewindAlarm();
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            var spacing = 1;
            loopBtn.Height = panel1.Height / 2 - spacing;
            replayBtn.Height = panel1.Height / 2 - spacing;
        }

        private void actionButtonsPanel_Resize(object sender, EventArgs e)
        {
            var diff = (actionButtonsPanel.Location.X + actionButtonsPanel.Width) - ackBtn.Location.X;
            ackBtn.Location = new Point(ackBtn.Location.X + diff, ackBtn.Location.Y);
            ackBtn.Size = new Size(ackBtn.Size.Width - diff, ackBtn.Size.Height);
        }

        private void actionButtonsPanel_ControlAdded(object sender, ControlEventArgs e)
        {
            if (actionButtonsPanel.Controls.Count % 2 == 0)
            {
                actionButtonsPanel.SetFlowBreak(e.Control as Control, true);

            }
        }

        public override bool ShowToolbar => false;
    }


}
