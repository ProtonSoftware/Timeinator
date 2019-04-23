using MvvmCross.Converters;
using System;
using System.Globalization;

namespace Timeinator.Mobile.Core
{
    /// <summary>
    /// Converts Paused session status to visibility
    /// </summary>
    public class PausedBoolToVisibility : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (bool)value;
            if ((bool)parameter)
                status = !status;
            return status ? "invisible" : "visible";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
