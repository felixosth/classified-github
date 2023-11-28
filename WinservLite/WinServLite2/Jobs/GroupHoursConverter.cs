using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using WinServLib.Objects;

namespace WinServLite2.Jobs
{
    public class GroupHoursConverter : IValueConverter
    {

        public object Convert(object value, System.Type targetType,
                              object parameter,
                              System.Globalization.CultureInfo culture)
        {
            if (null == value)
                return "null";

            ReadOnlyObservableCollection<object> items =
                  (ReadOnlyObservableCollection<object>)value;

            var hours = (from i in items
                         select (((TimeReport)i).WorkTime + ((TimeReport)i).TravelTime)).Sum();

            return "Total Time: " + hours.ToString() + "h";
        }

        public object ConvertBack(object value, System.Type targetType,
                                  object parameter,
                                  System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
