using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SelectAid.Views;

public sealed class BooleanToBrushConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var active = value is bool flag && flag;
        return active ? Brushes.Red : Brushes.Transparent;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return false;
    }
}
