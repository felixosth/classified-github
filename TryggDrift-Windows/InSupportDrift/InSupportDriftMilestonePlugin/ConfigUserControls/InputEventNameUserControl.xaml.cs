using System.Windows;
using System.Windows.Controls;

namespace InSupportDriftMilestonePlugin
{
    /// <summary>
    /// Interaction logic for InputEventNameUserControl.xaml
    /// </summary>
    public partial class InputEventNameUserControl : UserControl
    {
        public InputEventNameUserControl()
        {
            InitializeComponent();
        }

        private void okBtn_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).DialogResult = true;
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).DialogResult = false;
        }
    }
}
