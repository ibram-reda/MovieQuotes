namespace MovieQuotes.Application.Features.Movies.Models;

public class MovieWithStudyMaterialCount
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int? Year { get; set; }
    public string? CoverUrl { get; set; } = string.Empty;
    public int StudyMaterialCount { get; set; } = 0;

}
