using System;
using System.Globalization;
using System.Windows.Data;

namespace VKAlpha.Conventers
{
    public class AudioTimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var timeSpan = (TimeSpan)value;
            if (timeSpan.Hours > 0)
                return timeSpan.ToString("h\\:mm\\:ss");
            return timeSpan.ToString("mm\\:ss");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = (string)value;
            if (!TimeSpan.TryParse(str, out var result))
                return null;
            return result;
        }
    }
}
