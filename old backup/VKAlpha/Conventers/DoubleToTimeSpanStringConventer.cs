using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VKAlpha.Conventers
{
    public class DoubleToTimeSpanStringConventer : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var totalSeconds = (double)value;
            if (totalSeconds > 3599)
                return TimeSpan.FromSeconds(totalSeconds).ToString("h\\:mm\\:ss");
            return TimeSpan.FromSeconds(totalSeconds).ToString("mm\\:ss");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}