using System;
using System.Windows;
using System.Windows.Controls;

namespace TryggDriftPingMonitorPlugin
{
    /// <summary>
    /// Interaction logic for PingItemUserControlWpf.xaml
    /// </summary>
    public partial class PingItemUserControlWpf : UserControl
    {
        public event EventHandler OnDelete;

        public PingItemUserControlWpf()
        {
            InitializeComponent();
        }

        private void delBtn_Click(object sender, RoutedEventArgs e)
        {
            OnDelete?.Invoke(this, new EventArgs());
        }
    }
}
