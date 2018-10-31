using System;
using System.Globalization;

namespace Timeinator.Mobile
{
    /// <summary>
    /// A converter that takes in a boolean 
    /// and inverts it
    /// </summary>
    public class BooleanInversionConverter : BaseValueConverter<BooleanInversionConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}