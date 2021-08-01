using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Globalization;

namespace Trials.GTC.Converters
{
    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dt = (DateTime)value;

            if (parameter is string)
                return dt.ToString((string)parameter, new CultureInfo("en-US"));

            if (dt > DateTime.Today)
                return "Today";

            if (dt > DateTime.Today.Subtract(TimeSpan.FromDays(2)))
                return "Yesterday";

            if (dt > DateTime.Today.Subtract(TimeSpan.FromDays(7)))
                return dt.ToString("dddd", new CultureInfo("en-US"));

            var returnValue = dt.ToString("d", new CultureInfo("en-US"));
            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
