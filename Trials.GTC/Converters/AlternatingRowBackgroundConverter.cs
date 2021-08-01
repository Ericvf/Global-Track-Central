
namespace Trials.GTC.Converters
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Globalization;
    using Trials.GTC.Framework;

    public class AlternatingRowBackgroundConverter : IValueConverter
    {
        public Brush NormalBrush { get; set; }
        public Brush AlternateBrush { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Panel element = (Panel)value;
            element.Loaded += Element_Loaded;
            return NormalBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private void Element_Loaded(object sender, RoutedEventArgs e)
        {
            Panel element = sender as Panel;
            ItemsControl parent = element.FindParent(x => x is ItemsControl) as ItemsControl;

            if (parent != null)
            {
                DependencyObject container = parent.ItemContainerGenerator.ContainerFromItem(element.DataContext);
                if (container != null)
                {
                    int index = parent.ItemContainerGenerator.IndexFromContainer(container);
                    if (index % 2 != 0) element.Background = AlternateBrush;
                }
            }
        }
    }
}
