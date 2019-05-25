using MvvmCross.Converters;
using System;
using System.Globalization;
using System.Resources;
using Timeinator.Core;

namespace Timeinator.Mobile.Core
{
    /// <summary>
    /// Converts specified property name to corresponding string value from localization resource
    /// </summary>
    public class StringValueConverter : IMvxValueConverter
    {
        private static ResourceManager ResourceManager = new ResourceManager(typeof(LocalizationResource));
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => ResourceManager.GetString(value.ToString(), culture);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
