using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.Collections;
using System.Windows;
using System.Text;


namespace Trials.GTC.Converters
{

    public class TimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            var ts = (TimeSpan)value;
            var sb = new StringBuilder();

            if (ts.Hours > 0)
                sb.Append(ts.Hours.ToString() + ":");

            if (ts.Minutes > 0)
                sb.Append(ts.Minutes.ToString().PadLeft(2, '0') + ":");

            sb.Append(ts.Seconds.ToString().PadLeft(2, '0') + ".");
            sb.Append(ts.Milliseconds.ToString().PadLeft(3, '0'));

            return sb.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }

}
