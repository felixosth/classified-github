using REIDSearchAgent.SearchAgent;
using REIDShared.NodeRED;
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
using VideoOS.Platform;
using VideoOS.Platform.Data;

namespace REIDSearchAgent.SearchToolbar.Add
{
    /// <summary>
    /// Interaction logic for AddUsrMainUsrControl.xaml
    /// </summary>
    public partial class AddUsrMainUsrControl : UserControl
    {

        public AddUsrMainUsrControl()
        {
            InitializeComponent();
        }

        public AddUsrMainUsrControl(IEnumerable<ReidSearchAgentResultData> results, bool edit = false)
        {
            InitializeComponent();

            var categories = ReidPluginSearchAgentDefinition.NodeRED.GetCategories();

            foreach (var result in results)
            {
                AddUserUsrControl addUsrControl = null;
                if (!edit)
                {
                    addUsrControl = new AddUserUsrControl(result.Item, result.TriggerTime);
                    addUsrControl.KeyTxtBox = result.PersonKey;
                    //addUsrControl.Margin = new Thickness(0, 0, 0, 10);
                    addUsrControl.categoryComboBox.ItemsSource = categories;
                    addUsrControl.categoryComboBox.SelectedIndex = 0;

                    //new Thread(() => SetImage(addUsrControl.image, result.TriggerTime, result.Item)).Start();

                }
                else
                {
                    addUsrControl = new AddUserUsrControl(result.Person, result.Item, result.TriggerTime);
                    addUsrControl.categoryComboBox.ItemsSource = categories;
                    addUsrControl.categoryComboBox.SelectedItem = categories.FirstOrDefault(c => c.id == result.Person.category);
                }
                stackPanel.Children.Add(addUsrControl);
            }
        }

        //void SetImage(Image image, DateTime trigger, VideoOS.Platform.Item item)
        //{
        //    JPEGVideoSource source = new JPEGVideoSource(item);
        //    source.Init();
        //    var frame = source.GetNearest(trigger) as JPEGData;

        //    ClientControl.Instance.CallOnUiThread(() =>
        //    {
        //        image.Source = frame.ConvertToBitmapSource();
        //    });
        //}

        private void okBtn_Click(object sender, RoutedEventArgs e)
        {
            (this.Parent as Window).DialogResult = true;
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            (this.Parent as Window).DialogResult = false;
        }

        public List<NodeREDPerson> GetInput()
        {
            //var dict = new Dictionary<string, string>();

            var persons = new List<NodeREDPerson>();

            foreach(AddUserUsrControl child in stackPanel.Children)
            {
                persons.Add(child.GetPerson());
                //persons.Add(new NodeREDPerson()
                //{
                //    key = child.KeyTxtBox,
                //    personName = child.NameTxtBox,
                //    category = (child.categoryComboBox.SelectedItem as NodeREDCategory).id ?? default,
                //});
                //dict.Add(child.KeyTxtBox, child.NameTxtBox);
            }

            return persons;
        }
    }
}
