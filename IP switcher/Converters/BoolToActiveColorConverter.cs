using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TTech.IP_Switcher.Converters;

public class BoolToActiveColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType,
        object parameter, CultureInfo culture)
    {
        var state = value as bool?;

        if (state.HasValue && state.Value)
            return new SolidColorBrush(Colors.LightGreen);
        else
            return new SolidColorBrush(Colors.OrangeRed);
    }

    public object ConvertBack(object value, Type targetType,
        object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
