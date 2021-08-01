using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.Collections;
using System.Windows;


namespace Trials.GTC.Converters
{

     public class BitmapImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, 
                                  object parameter, CultureInfo culture) 
            { 
                try 
                { 
                    Uri source= new Uri(value.ToString()); 
                    return new BitmapImage(source); 
                } 
                catch(Exception) 
                { 
                    return new BitmapImage(); 
                } 
            }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

     }

  

    public class VisibilityConverter : IValueConverter
    {
        public bool Inversed { get; set; }
        public Visibility FalseValue { get; set; }

        public VisibilityConverter()
        {
            this.FalseValue = Visibility.Collapsed;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool visibility = false;

            if (value is bool)
                visibility = (bool)value;

            if (value is string)
                visibility = !string.IsNullOrEmpty((string)value);

            if (value is BitmapImage && value != null)
                visibility = true;

            if (value is IList)
                visibility = (value as IList).Count > 0;

            if (value is TimeSpan)
                visibility = ((TimeSpan)value).Ticks > 0;

            if (Inversed)
                visibility = !visibility;


            return visibility ? Visibility.Visible : this.FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visibility = (Visibility)value;
            return (visibility == Visibility.Visible);
        }
    }
}
