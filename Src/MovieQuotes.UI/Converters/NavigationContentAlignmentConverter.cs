namespace MovieQuotes.UI.Converters;

using Avalonia.Data.Converters;
using Avalonia.Layout;
using System;
using System.Globalization;

public sealed class NavigationContentAlignmentConverter : IValueConverter
{
    public static NavigationContentAlignmentConverter Instance { get; } = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is true
            ? HorizontalAlignment.Left
            : HorizontalAlignment.Center;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
