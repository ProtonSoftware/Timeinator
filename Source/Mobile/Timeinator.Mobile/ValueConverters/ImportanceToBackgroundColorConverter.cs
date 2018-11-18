using System;
using Xamarin.Forms;
using System.Globalization;

namespace Timeinator.Mobile
{
    /// <summary>
    /// A converter that takes in a task importance boolean and returns background color based on it 
    /// </summary>
    public class ImportanceToBackgroundColorConverter : BaseValueConverter<ImportanceToBackgroundColorConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var importance = (bool)value;

            if (importance)
            {
                return Color.LightPink;
            }
            else
            {
                return Color.White;
            }
        }


        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}