using System;
using System.Globalization;

namespace Timeinator.Mobile
{
    /// <summary>
    /// A converter that takes in a <see cref="ApplicationIconType"/> 
    /// and returns the MaterialDesignIcon string for that icon
    /// </summary>
    public class ApplicationIconTypeToMaterialFontConverter : BaseValueConverter<ApplicationIconTypeToMaterialFontConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((ApplicationIconType)value).ToMaterialFont();
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}