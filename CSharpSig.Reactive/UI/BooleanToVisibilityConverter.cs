using System;
using System.Globalization;

namespace ImprovingU.Reactive.UI
{
    public sealed class BooleanToVisibilityConverter : ValueConverterMarkupExtension<BooleanToVisibilityConverter>
    {
        readonly System.Windows.Controls.BooleanToVisibilityConverter _converter = 
            new System.Windows.Controls.BooleanToVisibilityConverter();

        public override object Convert(object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            return _converter.Convert(value, targetType, parameter, culture);
        }

        public override object ConvertBack(object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            return _converter.ConvertBack(value, targetType, parameter, culture);
        }
    }
}
