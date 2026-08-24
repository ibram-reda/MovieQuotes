namespace MovieQuotes.UI.Converters;

using System;
using System.Globalization;
using Avalonia.Data.Converters;

public class StudyProgressTypeIconConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => string.Equals(value?.ToString(), "Recognition", StringComparison.OrdinalIgnoreCase)
            ? "🧠"
            : "🎯";

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
