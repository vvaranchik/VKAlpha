using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VKAlpha.Conventers
{
    public class NullToVisibilityConventer : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var invert = false;
            if (parameter != null)
            {
                bool.TryParse(parameter.ToString(), out invert);
            }
            if (value == null) return invert ? Visibility.Visible : Visibility.Collapsed;

            if (bool.TryParse(value.ToString(), out bool result))
            {
                if (invert)
                {
                    result = !result;
                    if (result)
                        return Visibility.Collapsed;
                    else
                        return Visibility.Visible;
                }
                else
                {
                    if (result)
                        return Visibility.Visible;
                    else
                        return Visibility.Collapsed;
                }

            }

            if (value is string)
                return string.IsNullOrWhiteSpace((string)value) || invert ? Visibility.Collapsed : Visibility.Visible;

            if (value is IList)
            {
                bool empty = ((IList)value).Count == 0;
                if (invert)
                    empty = !empty;
                if (empty)
                    return Visibility.Collapsed;
                else
                    return Visibility.Visible;
            }

            decimal number;
            if (decimal.TryParse(value.ToString(), out number))
            {
                if (!invert)
                    return number > 0 ? Visibility.Visible : Visibility.Collapsed;
                else
                    return number > 0 ? Visibility.Collapsed : Visibility.Visible;

            }

            return invert ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
