namespace MovieQuotes.UI.Converters;

using Avalonia.Data.Converters;
using System;
using System.Globalization;

internal class LongToTimeConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is long ms)
            return TimeSpan.FromMilliseconds(ms).ToString(@"hh\:mm\:ss");
        throw new NotImplementedException();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
