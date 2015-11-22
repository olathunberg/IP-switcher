using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Effects;

namespace Deucalion.IP_Switcher.Converters
{
    public class BoolToBlurEffectConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
             var state = value as bool?;

             if (state.HasValue && state.Value)
                 return new BlurEffect();
             else
                 return null;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
