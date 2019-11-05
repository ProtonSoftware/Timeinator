using MvvmCross.Converters;
using System;
using System.Globalization;

namespace Timeinator.Mobile.Android
{
    /// <summary>
    /// Converts double Progress to int or string if parameter provided
    /// </summary>
    public class ProgressValueConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var progress = (int)((double)value * 100);
            if (parameter != null)
                return $"{progress}%";
            else
                return progress;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
