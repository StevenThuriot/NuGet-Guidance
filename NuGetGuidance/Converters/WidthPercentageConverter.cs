using System;
using System.Globalization;
using System.Windows.Data;

namespace NuGetGuidance.Converters
{
    public class WidthPercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var percentage = Double.Parse(parameter.ToString(), CultureInfo.InvariantCulture);
            return ((double)value) * percentage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
