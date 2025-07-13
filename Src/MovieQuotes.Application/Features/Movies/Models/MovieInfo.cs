namespace MovieQuotes.Application.Features.Movies.Models;

public class MovieInfo
{
    /// <summary>
    /// movie id.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// movie title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// movie description.
    /// </summary>
    public string? Description { get; set; }

    public string BaseFolderDir { get; set; } = string.Empty;

    /// <summary>
    /// Video location.
    /// </summary>
    public string LocalPath { get; set; } = string.Empty;

    /// <summary>
    /// IMDB ID
    /// </summary>
    public string? IMDBId { get; set; } = string.Empty;

    /// <summary>
    /// Subtitle location.
    /// </summary>
    public string? SubtitlePath { get; set; } = string.Empty;

    /// <summary>
    /// cover path.
    /// </summary>
    public string? CoverUrl { get; set; } = string.Empty;

    /// <summary>
    /// movie production year.
    /// </summary>
    public int? Year { get; set; }
    public string FolderName { get; set; } = string.Empty;
}