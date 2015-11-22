using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Deucalion.IP_Switcher.Converters
{
    public class BoolToFontWeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
             var state = value as bool?;

             if (state.HasValue && state.Value)
                 return FontWeights.Bold;
             else
                 return FontWeights.Normal;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
