namespace MovieQuotes.Application.Features.Movies.Queries;

using System.Text.RegularExpressions;

public static class MovieSearchParser
{
    private static readonly Regex FilterRegex =
        new(@"(?<key>\w+)(?<op>:|>=|<=|>|<)(?<value>""[^""]+""|\S+)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static MovieSearchFilters Parse(string search)
    {
        var filters = new MovieSearchFilters();

        if (string.IsNullOrWhiteSpace(search))
            return filters;

        var matches = FilterRegex.Matches(search);

        foreach (Match match in matches)
        {
            var key = match.Groups["key"].Value.ToLower();
            var op = match.Groups["op"].Value;
            var value = match.Groups["value"].Value.Trim('"');

            switch (key)
            {
                case "imdb":
                    filters.ImdbId = value;
                    break;

                case "genre":
                    filters.Genre = value;
                    break;

                case "year":
                    ParseYear(filters, op, value);
                    break;

                case "rating":
                    ParseRating(filters, op, value);
                    break;
            }
        }

        // Remove filter expressions to get free text
        var freeText = FilterRegex.Replace(search, "").Trim();

        filters.Text = string.IsNullOrWhiteSpace(freeText)
            ? null
            : freeText;

        return filters;
    }

    private static void ParseYear(
        MovieSearchFilters filters,
        string op,
        string value)
    {
        if (!int.TryParse(value, out var year))
            return;

        switch (op)
        {
            case ":":
                filters.Year = year;
                break;

            case ">":
            case ">=":
                filters.MinYear = year;
                break;

            case "<":
            case "<=":
                filters.MaxYear = year;
                break;
        }
    }

    private static void ParseRating(
        MovieSearchFilters filters,
        string op,
        string value)
    {
        if (!double.TryParse(value, out var rating))
            return;

        switch (op)
        {
            case ">":
            case ">=":
                filters.MinRating = rating;
                break;

            case "<":
            case "<=":
                filters.MaxRating = rating;
                break;
        }
    }
}