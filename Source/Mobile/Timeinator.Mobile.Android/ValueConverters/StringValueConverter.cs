using MvvmCross.Converters;
using System;
using System.Globalization;
using System.Resources;
using Timeinator.Core;

namespace Timeinator.Mobile.Android
{
    /// <summary>
    /// Converts specified property name to corresponding string value from localization resource
    /// </summary>
    public class StringValueConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Get localization resource and find the appropriate string based on provided value
            // Culture is not needed to be provided here, because localization resource itself is set to current culture already
            var resourceManager = new ResourceManager(typeof(LocalizationResource));
            return resourceManager.GetString(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
