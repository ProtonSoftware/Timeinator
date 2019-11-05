using MvvmCross.Converters;
using System;
using System.Globalization;
using Timeinator.Mobile.Domain;

namespace Timeinator.Mobile.Android
{
    /// <summary>
    /// The value converter for task's type to get appropriate icon for
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
