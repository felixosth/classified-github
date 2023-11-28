using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WinservLite
{
    partial class ThemeEventHandler
    {
        public ThemeEventHandler()
        {
        }

        void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // find daddy
            var item = sender as ListViewItem;
            try
            {


                var main = (ItemsControl.ItemsControlFromItemContainer(item).Parent as Grid).Parent;
                var typeString = main.GetType().ToString();
                switch (main.GetType().ToString())
                {
                    case "WinservLite.MainWindow":
                        (main as MainWindow).ListViewItem_MouseDoubleClick(sender, e);
                        break;
                        //case "DisplayJob":
                        //    //(main as DisplayJob).ListViewItem_MouseDoubleClick(sender, e);

                        //    break;
                }

            }
            catch
            {
                var main = (ItemsControl.ItemsControlFromItemContainer(item).Parent as Expander).Parent as MainWindow;
                main.bookmarkList_ItemDoubleClick(sender, e);

            }


            //var src = e.OriginalSource;
        }
    }
}
