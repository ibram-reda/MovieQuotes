namespace MovieQuotes.Application.Features.Movies.Queries;

public class MovieSearchFilters
{
    public string? Text { get; set; }

    public string? ImdbId { get; set; }

    public int? Year { get; set; }

    public int? MinYear { get; set; }

    public int? MaxYear { get; set; }

    public string? Genre { get; set; }

    public double? MinRating { get; set; }

    public double? MaxRating { get; set; }
}