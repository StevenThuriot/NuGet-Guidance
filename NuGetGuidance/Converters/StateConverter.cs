using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace NuGetGuidance.Converters
{
    internal class StateConverter : IMultiValueConverter
    {
        public SolidColorBrush IsExecutingResource { get; set; }
        public SolidColorBrush IsWaitingResource { get; set; }
        public SolidColorBrush IsInErrorResource { get; set; }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var isExecuting = values[0] as bool?;
            if (isExecuting.GetValueOrDefault()) return IsExecutingResource;
            var isInError = values[1] as bool?;
            if (isInError.GetValueOrDefault()) return IsInErrorResource;

            return IsWaitingResource;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return Enumerable.Repeat(Binding.DoNothing, targetTypes.Length).ToArray();
        }
    }
}
