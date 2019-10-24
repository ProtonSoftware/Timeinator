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
            var priority = (Priority)value;

            return $"P: {(int)priority}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
