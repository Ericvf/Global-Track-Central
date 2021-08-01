using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Linq;
using System.Windows.Data;
using System.Globalization;
using System.Text;
using System.Collections.ObjectModel;

namespace Trials.GTC.Converters
{
    public class TagsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ObservableCollection<Guid>)
            {
                try
                {
                    var tags = App.tags.Where(t => ((ObservableCollection<Guid>)value).Contains(t.Id)).ToArray();
                    var sb = new StringBuilder();

                    for (int i = 0; i < tags.Length; i++)
                    {
                        if (tags[i].IsCompetition)
                            continue;

                        sb.Append(tags[i].Name);
                        if (i < tags.Length - 1)
                            sb.Append(", ");
                    }

                    return sb.ToString();
                }
                catch { }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
