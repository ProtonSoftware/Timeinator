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
            var type = (TimeTaskType)value;

            switch(type)
            {
                case TimeTaskType.Generic:
                    return Resource.Drawable.icon_type_generic;

                case TimeTaskType.Reading:
                    return Resource.Drawable.icon_type_reading;

                default:
                    return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
