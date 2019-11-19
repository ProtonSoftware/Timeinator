using MvvmCross.Converters;
using System;
using System.Globalization;
using Timeinator.Core;

namespace Timeinator.Mobile.Android
{
    /// <summary>
    /// The value converter for <see cref="TimeSpan"/> to show as human-friendly text
    /// Example: 1h 19m 0s instead of 01:19:00
    /// </summary>
    public class TimeSpanToTextValueConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Get the provided time value
            var timeSpan = (TimeSpan)value;

            // If parameter was provided, prepend a text before time values
            var prependSentence = parameter != null ? LocalizationResource.EstimatedSessionTimeSemicolon : "";

            // Return formatted string
            return $"{prependSentence}{timeSpan.Hours}h {timeSpan.Minutes}m {timeSpan.Seconds}s";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
