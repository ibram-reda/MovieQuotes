namespace MovieQuotes.UI.Converters;

using System;
using System.Globalization;
using Avalonia.Data.Converters;

public class StudyReviewDateConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not DateTime reviewDate)
            return string.Empty;

        var today = DateTime.Today;
        var reviewDay = reviewDate.Date;
        var daysUntilReview = (reviewDay - today).Days;

        return daysUntilReview switch
        {
            <= 0 => "↻ Due now",
            1 => "↻ Tomorrow",
            <= 7 => $"↻ In {daysUntilReview} days",
            _ => $"↻ {reviewDate.ToString("d", culture)}"
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
