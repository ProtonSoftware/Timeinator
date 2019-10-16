using MvvmCross.Converters;
using System;
using System.Globalization;
using Timeinator.Core;

namespace Timeinator.Mobile.Domain
{
    /// <summary>
    /// The value converter for task's priority to show it as text instead of enum name
    /// </summary>
    public class PriorityEnumToTextValueConverter : IMvxValueConverter
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
