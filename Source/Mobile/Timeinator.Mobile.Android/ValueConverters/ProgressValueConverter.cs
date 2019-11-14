using MvvmCross.Converters;
using System;
using System.Globalization;

namespace Timeinator.Mobile.Android
{
    /// <summary>
    /// Converts progress value to percentage int or to percentage string in case parameter was provided
    /// </summary>
    public class ProgressValueConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Get the progress as integer 0-100% value
            var progress = (int)((double)value * 100);

            // Return it as string if parameter was provided
            if (parameter != null)
                return $"{progress}%";

            // Otherwise, return as int
            return progress;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
