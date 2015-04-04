using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Shell;

namespace NuGetGuidance.Converters
{
    internal class BoolToProgressStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value == DependencyProperty.UnsetValue || (!(bool)value))
                return TaskbarItemProgressState.None;

            return TaskbarItemProgressState.Indeterminate;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
