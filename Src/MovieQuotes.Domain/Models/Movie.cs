namespace MovieQuotes.Domain.Models;

using MovieQuotes.Domain.Exception;
using MovieQuotes.Domain.Validators;
using System.Text.RegularExpressions;

public class Movie
{
    private Movie() { }
    public int Id { get; private set; }
    public string NameId { get; set; } = string.Empty;
    public string Title { get;private set; } = string.Empty;
    public string? Description { get; private set; }

    public string BaseFolderDir { get; private set; } = string.Empty;
    public string LocalPath { get; private set; } = string.Empty;
    public string? CoverUrl { get; private set; } = string.Empty;

    public int Year { get;  set; } = 0;
    public string? IMDBId { get; private set; }

    public DateTime AddedDate { get; private set; }
    public List<SubtitlePhrase> Subtitles { get; } = new();

    /// <summary>
    /// Create a new Movie Object.
    /// </summary>
    /// <param name="title">Movie name.</param>
    /// <param name="localPath">video path.</param>
    /// <param name="description">movie description.</param>
    /// <returns>instance of <see cref="Movie"/>.</returns>
    /// <exception cref="MovieNotValidException"></exception>
    public static Movie CreateMovie(string baseFolder,string title,string localPath ,string? description,string? IMDBID,string coverUrl,int year)
    {
        var validator = new MovieValidator();

        var movie = new Movie()
        {
            BaseFolderDir = baseFolder,
            NameId = TitleToNameId(title),
            Title = title,
            Description = description,
            LocalPath = localPath,
            IMDBId = IMDBID,
            CoverUrl = coverUrl,
            AddedDate = DateTime.Now,
            Year = year
        }; 

        var validationResult = validator.Validate(movie);

        if(validationResult.IsValid) return movie;

        var exception = new MovieNotValidException("Movie is not valid");
        exception.ValidationErrors.AddRange(validationResult.Errors.Select(a => a.ErrorMessage));
         
        throw exception;
    } 

    /// <summary>
    /// Add list of <see cref="SubtitlePhrase"/> to movie.
    /// </summary>
    /// <param name="subtitlePhrases"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void AddSubtitlesFromList(List<SubtitlePhrase> subtitlePhrases)
    {
        if (Subtitles.Count > 0)
            throw new InvalidOperationException("this movie already has subtitles");

        Subtitles.AddRange(subtitlePhrases);
    }
    
    private static string TitleToNameId(string title)
    {
        var n = new Regex(@"\s+").Replace(title, "-");
        return new Regex(@"[^a-zA-Z\d-]").Replace(n,"");
    }
}
