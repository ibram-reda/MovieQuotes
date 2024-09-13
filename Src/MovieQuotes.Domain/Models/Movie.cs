namespace MovieQuotes.Domain.Models;

using MovieQuotes.Domain.Exception;
using MovieQuotes.Domain.Validators;

public class Movie
{
    private Movie() { }
    public int Id { get; private set; }
    public string Title { get;private set; } = string.Empty;
    public string? Description { get; private set; }
    public string LocalPath { get; private set; } = string.Empty;

    public List<SubtitlePhrase> Subtitles { get; } = new();

    /// <summary>
    /// Create a new Movie Object.
    /// </summary>
    /// <param name="title">Movie name.</param>
    /// <param name="localPath">video path.</param>
    /// <param name="description">movie description.</param>
    /// <returns>instance of <see cref="Movie"/>.</returns>
    /// <exception cref="MovieNotValidException"></exception>
    public static Movie CreateMovie(string title,string localPath ,string? description)
    {
        var validator = new MovieValidator();

        var movie = new Movie() {
            Title = title,
            Description = description,
            LocalPath = localPath,
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
    /// <param name="stream">stream where phrase come from.</param>
    /// <returns>async Task that will add phrase to movie.</returns>
    /// <exception cref="InvalidOperationException">if the movie already has been initialized before</exception>
    public async Task AddSubtitlesFromStreamAsync(StreamReader stream)
    {         
        var phrases = await SubtitlePhrase.GetPhrasesFromStreamAsync(stream);

        AddSubtitlesFromList(phrases);
    }

    private void AddSubtitlesFromList(List<SubtitlePhrase> subtitlePhrases)
    {
        if (Subtitles.Count > 0)
            throw new InvalidOperationException("this movie already has subtitles");


        Subtitles.AddRange(subtitlePhrases);
    }


    public async Task AddSubtitleFromFileAsync(string subtitleFilePath)
    {
        using var fileStream = File.OpenRead(subtitleFilePath);
        using var reader = new StreamReader(fileStream);
        await AddSubtitlesFromStreamAsync(reader);
    }
}
