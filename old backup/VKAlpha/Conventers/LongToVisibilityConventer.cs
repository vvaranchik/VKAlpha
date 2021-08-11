using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace VKAlpha.Conventers
{
    public class LongToVisibilityConventer : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = long.Parse(value.ToString());
            bool invert = false;
            if (parameter != null) invert = Boolean.Parse(parameter.ToString());
            if (Helpers.MainViewModelLocator.Vk.AccessToken.UserId == result)
            {
                if (!invert)
                    return Visibility.Visible;
                return Visibility.Collapsed;
            }
            if (!invert)
                return Visibility.Collapsed;
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
