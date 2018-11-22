using System;
using System.Globalization;

namespace Timeinator.Mobile
{
    /// <summary>
    /// A converter that takes in a boolean 
    /// and inverts it
    /// </summary>
    public class DoubleToPercentConverter : BaseValueConverter<DoubleToPercentConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value * 100;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value / 100;
        }
    }
}