using MvvmCross.Converters;
using System;
using System.Globalization;

namespace Timeinator.Mobile.Android
{
    /// <summary>
    /// Converts provided progress to fraction string
    /// </summary>
    public class ProgressFractionValueConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var currentProgress = (double)value;
            var maxProgress = (double)parameter;

            return $"{currentProgress} / {maxProgress}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
