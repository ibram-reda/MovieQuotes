namespace MovieQuotes.Application.Models;

public class MovieFullInfo
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string LocalPath { get; set; } = string.Empty;
    public string? IMDBId { get; set; } = string.Empty;

    public string? SubtitlePath { get; set; } = string.Empty;
    public string? CoverUrl { get; set; } = string.Empty;
    public int? Year { get; set; }

    public List<Phrase> phrases { get; set; } = [];
}
