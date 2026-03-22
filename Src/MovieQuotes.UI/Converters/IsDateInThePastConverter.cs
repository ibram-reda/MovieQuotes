namespace MovieQuotes.UI.Converters;

using Avalonia.Data.Converters;
using System;
using System.Globalization;

internal class IsDateInThePastConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime dateTime)
            return dateTime <= DateTime.Now;
        throw new NotImplementedException();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
