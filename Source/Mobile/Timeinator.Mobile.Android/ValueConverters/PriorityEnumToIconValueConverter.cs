using MvvmCross.Converters;
using System;
using System.Globalization;
using Timeinator.Mobile.Domain;

namespace Timeinator.Mobile.Android
{
    /// <summary>
    /// The value converter for task's priority to show it as icon
    /// </summary>
    public class PriorityEnumToIconValueConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Get the actual priority value
            var priority = (Priority)value;

            // Return it as icon
            return priority.ToIcon();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
