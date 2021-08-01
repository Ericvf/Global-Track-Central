using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Globalization;

namespace Trials.GTC.Converters
{
    public class DifficultyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = (int)value;
            
            var xxx = App.difficulties.FirstOrDefault(ver => ver.Id.Equals(v));
            if (xxx != null)
                return xxx.Name;
            //switch (v)
            //{
            //    case 0:
            //        return "Beginner";

            //    case 1:
            //        return "Easy";

            //    case 2:
            //        return "Medium";

            //    case 3:
            //        return "Hard";

            //    case 4:
            //        return "Extreme";

            //    case 5:
            //    case 6:
            //    case 7:
            //    case 8:
            //    case 9:
            //        return "Ninja " + ((int)v - 4).ToString();     
            //}

            return v;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
