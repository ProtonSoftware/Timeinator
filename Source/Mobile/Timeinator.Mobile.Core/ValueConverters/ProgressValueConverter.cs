using MvvmCross.Converters;
using System;
using System.Globalization;

namespace Timeinator.Mobile.Core
{
    /// <summary>
    /// Converts double Progress to int or string if parameter provided
    /// </summary>
    public class ProgressValueConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var progress = (double)value * 100;
            if (parameter != null)
                return string.Format("{0:0.00}%", progress);
            else
                return (int)progress;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
