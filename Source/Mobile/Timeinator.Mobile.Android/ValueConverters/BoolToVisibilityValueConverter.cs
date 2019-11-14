using MvvmCross.Converters;
using System;
using System.Globalization;

namespace Timeinator.Mobile.Android
{
    /// <summary>
    /// Converts bool flag value to visibility string
    /// </summary>
    public class BoolToVisibilityValueConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Catch the bool value
            var isVisible = (bool)value;

            // If parameter was provided...
            if (parameter != null)
                // Invert the flag
                isVisible ^= true;

            // Return visibility string
            return isVisible ? "visible" : "invisible";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
