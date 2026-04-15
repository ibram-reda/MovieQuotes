namespace MovieQuotes.Infrastructure.Tests;

using MovieQuotes.Infrastructure.Data;
using MovieQuotes.Domain.Models;


public class DbFixture : IDisposable
{
    public MovieQuotesDbContext Context { get; }
    public DbFixture()
    {
        var factory= new MovieQoutesContextFatory();
        Context = factory.CreateDbContext(Array.Empty<string>());

        Context.Database.EnsureDeleted();
        Context.Database.EnsureCreated();
        SeedData();
    }

    public void Dispose()
    {
        Context.Dispose();
    }

    public void SeedData()
    {
        var movies = new List<Movie>
        {
            // public static Movie CreateMovie(string baseFolder,string folderName,string title,string localPath ,string? description,string? IMDBID,string coverUrl,int year)
            Movie.CreateMovie("/movies", "shawshank (1994)", "The Shawshank Redemption", "/movies/shawshank.1994.mp4", null, "tt0111161", "cover.jpg", 1994),
            Movie.CreateMovie("/movies", "godfather (1972)", "The Godfather", "/movies/godfather.1972.mp4", null, "tt0068646", "cover.jpg", 1972),
            Movie.CreateMovie("/movies", "dark-knight (2008)", "The Dark Knight", "/movies/dark-knight.2008.mp4", null, "tt0468569", "cover.jpg", 2008)
        };

        Context.Movies.AddRange(movies);
        Context.SaveChanges();

        var subs1 = new List<SubtitlePhrase>
        {
            SubtitlePhrase.CreateSubtitlePhrase(1, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(2), "Hello, world!"),
            SubtitlePhrase.CreateSubtitlePhrase(2, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(5), "Welcome to the movie."),
            SubtitlePhrase.CreateSubtitlePhrase(3, TimeSpan.FromSeconds(6), TimeSpan.FromSeconds(8), "Enjoy watching!")
        };

        var subs2 = new List<SubtitlePhrase>
        {
            SubtitlePhrase.CreateSubtitlePhrase(1, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(2), "This is the Godfather."),
            SubtitlePhrase.CreateSubtitlePhrase(2, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(5), "An iconic movie."),
            SubtitlePhrase.CreateSubtitlePhrase(3, TimeSpan.FromSeconds(6), TimeSpan.FromSeconds(8), "A must-watch!")
        };

        Context.Movies.First().AddSubtitlesFromList(subs1);
        Context.Movies.Skip(1).First().AddSubtitlesFromList(subs2);        
        Context.SaveChanges();
    }
}