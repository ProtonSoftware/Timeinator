using MvvmCross.Converters;
using System;
using System.Globalization;
using System.Resources;
using Timeinator.Core;

namespace Timeinator.Mobile.Domain
{
    /// <summary>
    /// Converts specified property name to corresponding string value from localization resource
    /// </summary>
    public class StringValueConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var resourceManager = new ResourceManager(typeof(LocalizationResource));
            return resourceManager.GetString(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
