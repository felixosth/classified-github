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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TryggSupport.ContactForm
{
    /// <summary>
    /// Interaction logic for ContactFormUserControl.xaml
    /// </summary>
    public partial class ContactFormUserControl : UserControl
    {
        List<TextBox> controlsToValidate;

        public ContactFormUserControl()
        {
            InitializeComponent();

            controlsToValidate = new List<TextBox>()
            {
                companyTxtBox, nameTxtBox,
                emailTxtBox, phoneTxtBox,
                messageTxtBox
            };
        }

        private bool ValidateControls(List<TextBox> controls)
        {
            bool foundError = false;
            string errors = "";

            foreach(var control in controls)
            {
                if(string.IsNullOrEmpty(control.Text))
                {
                    InvalidateControl(control);
                    errors += "\r\n" + control.Tag;
                    foundError = true;
                }
            }
            //grid.Refresh();

            if(foundError)
                MessageBox.Show("Var god fyll i följande fält:" + errors, "Kontaktformulär", MessageBoxButton.OK, MessageBoxImage.Information);

            return !foundError;
        }

        private void InvalidateControl(TextBox textBox)
        {
            textBox.BorderBrush = new SolidColorBrush(Colors.Red);
            textBox.Background = Brushes.Green;
        }

        private void sendBtn_Click(object sender, RoutedEventArgs e)
        {
            if(ValidateControls(controlsToValidate))
            {

            }
        }
    }
}
