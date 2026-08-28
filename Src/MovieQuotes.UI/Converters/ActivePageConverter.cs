namespace MovieQuotes.UI.Converters;

using Avalonia.Data.Converters;
using System;
using System.Globalization;

public sealed class ActivePageConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return string.Equals(value as string, parameter as string, StringComparison.Ordinal);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
