using System;
using System.Collections;
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
using VideoOS.Platform.Search;

namespace UVAPSearchAgent.Shared.Filters.RadioList
{
    // Custom user control for the radio list user control for search filters.
    // This class is created because the API lacks a radio list user control
    public partial class RadioListFilterUserControl : SearchFilterEditControl
    {
        RadioListFilterValue RadioValue => FilterValue as RadioListFilterValue;
        bool init = false;
        public RadioListFilterUserControl(RadioListFilterConfiguration config)
        {
            InitializeComponent();
            listBox.ItemsSource = config.Items;
        }

        public override void Init()
        {
            base.Init();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!init)
                return;

            var radio = sender as RadioButton;
            RadioValue.SelectedObject = radio.Content;
            RadioValue.SelectedObjectString = radio.Content.ToString();
        }
        
        private void SearchFilterEditControl_Loaded(object sender, RoutedEventArgs e)
        {
            init = false;
            foreach(var item in listBox.ItemsSource)
            {
                if(item.ToString() == RadioValue.SelectedObjectString)
                {
                    var container = listBox.ItemContainerGenerator.ContainerFromItem(item);
                    FindVisualChild<RadioButton>(container).IsChecked = true;
                    break;
                }
            }
            init = true;
        }

        private ChildItem FindVisualChild<ChildItem>(DependencyObject obj) where ChildItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is ChildItem)
                    return (ChildItem)child;
                else
                {
                    ChildItem childOfChild = FindVisualChild<ChildItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }
    }
}
