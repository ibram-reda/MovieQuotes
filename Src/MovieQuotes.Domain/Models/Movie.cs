namespace MovieQuotes.Domain.Models;

using MovieQuotes.Domain.Exception;
using MovieQuotes.Domain.Validators;
using System.Text.RegularExpressions;

public class Movie
{
    private Movie() { }
    public int Id { get; private set; }
    public string NameId { get; set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public int Year { get; set; } = 0;
    public string? IMDBId { get; private set; }

    public string BaseFolderDir { get; private set; } = string.Empty;
    public string FolderName { get; private set; } = string.Empty;
    public string VideoFilePath { get; private set; } = string.Empty;
    public string CoverFilePath { get; private set; } = string.Empty;
    public string? BackdropFilePath { get; private set; } = string.Empty;
    public string? PosterFilePath { get; private set; } = string.Empty;
    public string? TMDbId { get; private set; } = string.Empty;

    public float VoteAverage { get; set; } = 0f;
    public int VoteCount { get; set; } = 0;
    public bool IsAdult { get; set; } = false;

    public virtual List<Genre> Genres { get; } = new();

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
    public static Movie CreateMovie(string baseFolder, string folderName, string title, string localPath, string? description, string? IMDBID, string coverUrl, int year)
    {
        var validator = new MovieValidator();

        var movie = new Movie()
        {
            BaseFolderDir = baseFolder,
            FolderName = folderName,
            NameId = TitleToNameId(title),
            Title = title,
            Description = description,
            IMDBId = IMDBID,
            AddedDate = DateTime.Now,
            Year = year,
            VideoFilePath = localPath,
            CoverFilePath = coverUrl,
        };

        var validationResult = validator.Validate(movie);

        if (validationResult.IsValid) return movie;

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

    public async Task GenerateInfoFileAsync(bool overwrite = false)
    {
        var info = new
        {
            IMDBId = this.IMDBId,
            TMDBID = this.TMDbId,
            Title = this.Title,
            Description = this.Description,
            Year = this.Year,
            BackdropFilePath = this.BackdropFilePath,
        };

        var infoFilePath = Path.Combine(BaseFolderDir, FolderName, $"{NameId}-info.json");
        if (File.Exists(infoFilePath))
        {
            if (!overwrite) return;
            File.Delete(infoFilePath);
        }
        var json = System.Text.Json.JsonSerializer.Serialize(info, new System.Text.Json.JsonSerializerOptions() { WriteIndented = true });
        await File.WriteAllTextAsync(infoFilePath, json);
    }


    public void UpdateTMDbData(string json)
    {         
        using var doc = System.Text.Json.JsonDocument.Parse(json);

        var results = doc.RootElement.GetProperty("movie_results");
        if (results.GetArrayLength() == 0)
        {
            Console.WriteLine($"Movie not found: {this.IMDBId}");
            return;
        }

        this.BackdropFilePath = results[0].GetProperty("backdrop_path").GetString()?.TrimStart('/');
        this.PosterFilePath = results[0].GetProperty("poster_path").GetString()?.TrimStart('/');
        this.TMDbId = results[0].GetProperty("id").GetInt32().ToString();  
        this.VoteAverage = results[0].GetProperty("vote_average").GetSingle();
        this.VoteCount = results[0].GetProperty("vote_count").GetInt32();
        this.IsAdult = results[0].GetProperty("adult").GetBoolean();
    }

    public async Task DownloadBackDropAsync()
    {
        if (string.IsNullOrEmpty(this.BackdropFilePath))
        {
            return;// NO backdrop TO CASH
        }
        var backdropFilePath = Path.Combine(BaseFolderDir, FolderName, $"{this.BackdropFilePath.TrimStart('/')}");
        if (File.Exists(backdropFilePath))
        {
            return; // already backdrop file EXISTS, so no need to download
        }
        var backdropUrl = $"https://image.tmdb.org/t/p/original/{this.BackdropFilePath}";
        using var client = new HttpClient();
        var bytes = await client.GetByteArrayAsync(backdropUrl);
        await File.WriteAllBytesAsync(backdropFilePath, bytes);
    }

     public async Task DownloadPosterAsync()
    {
        if (string.IsNullOrEmpty(this.PosterFilePath))
        {
            return;// NO poster TO CASH
        }
        var posterFilePath = Path.Combine(BaseFolderDir, FolderName, $"{this.PosterFilePath.TrimStart('/')}");
        if (File.Exists(posterFilePath))
        {
            return; // already poster file EXISTS, so no need to download
        }
        var posterUrl = $"https://image.tmdb.org/t/p/original/{this.PosterFilePath}";
        using var client = new HttpClient();
        var bytes = await client.GetByteArrayAsync(posterUrl);
        await File.WriteAllBytesAsync(posterFilePath, bytes);
    }

    private static string TitleToNameId(string title)
    {
        var n = new Regex(@"\s+").Replace(title, "-");
        return new Regex(@"[^a-zA-Z\d-]").Replace(n, "");
    }

    public void AddGenre(Genre genre)
    {
        if (!Genres.Any(g => g.Id == genre.Id))
        {
            Genres.Add(genre);
        }
    }

    public string GetVideoPath()
    {
        return Path.Combine(BaseFolderDir, FolderName, VideoFilePath);
    }
    public string GetCoverUrl()
    {
        return Path.Combine(BaseFolderDir, FolderName, CoverFilePath);
        
    }
}
