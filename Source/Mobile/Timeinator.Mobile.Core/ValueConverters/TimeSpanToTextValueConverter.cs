﻿using MvvmCross.Converters;
using System;
using System.Globalization;

namespace Timeinator.Mobile.Core
{
    /// <summary>
    /// The value converter for <see cref="TimeSpan"/> to show as human-friendly text
    /// Example: 1h 19m 0s instead of 01:19:00
    /// </summary>
    public class TimeSpanToTextValueConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var timespan = (TimeSpan)value;

            return $"{timespan.Hours}h {timespan.Minutes}m {timespan.Seconds}s";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
