using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WinServLite2.CloseableTab
{
    /// <summary>
    /// Interaction logic for ContainerWindow.xaml
    /// </summary>
    public partial class ContainerWindow : Window
    {
        public ContainerWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            bool cancel = false;
            var content = this.Content as DynamicUserControl;
            if (content != null)
                content.OnClosing(out cancel);

            e.Cancel = cancel;
        }
    }
}
