using MvvmCross.Converters;
using System;
using System.Globalization;
using Timeinator.Mobile.Domain;

namespace Timeinator.Mobile.Android
{
    /// <summary>
    /// Converts task's type to associated icon to show in UI
    /// </summary>
    public class TimeTaskTypeToIconValueConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Get the actual task type
            var type = (TimeTaskType)value;

            // Return it as icon
            return type.ToIcon();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
