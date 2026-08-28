namespace MovieQuotes.UI.Converters;

using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Styling;
using System;
using System.Globalization;

public sealed class NavigationIconConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string resourceKey && Application.Current is { } application &&
            application.TryGetResource(resourceKey, ThemeVariant.Default, out var icon))
        {
            return icon;
        }

        return null;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
