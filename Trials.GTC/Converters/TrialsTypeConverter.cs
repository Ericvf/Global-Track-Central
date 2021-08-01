using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
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
    public class TrialsTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = (int)value;


            var xxx = App.trialsTypes.FirstOrDefault(ver => ver.Id.Equals(v));
            if (xxx != null)
                return xxx.Name;

            //switch (v)
            //{
            //    case 0:
            //        return "Trials";

            //    case 1:
            //        return "Supercross";

            //    case 2:
            //        return "Skillgame";
            //}

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
