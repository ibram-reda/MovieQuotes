namespace MovieQuotes.Application.Features.Movies.Mappings;

using MovieQuotes.Application.Features.Movies.Models;
using MovieQuotes.Domain.Models;


internal static class MovieMap
{
    internal static MovieInfo ToMovieInfo(this Movie movie)
    {
        MovieInfo result = new MovieInfo();
        result.Id = movie.Id;
        result.Title = movie.Title;
        result.LocalPath = Path.Combine( movie.BaseFolderDir, movie.FolderName, movie.VideoFilePath ?? string.Empty);
        result.IMDBId = movie.IMDBId;
        result.Description = movie.Description;
        result.CoverUrl = Path.Combine( movie.BaseFolderDir, movie.FolderName, movie.CoverFilePath ?? string.Empty);
        result.BaseFolderDir = movie.BaseFolderDir;
        result.FolderName = movie.FolderName;
        result.Year = movie.Year;
        return result;
    }

    internal static MovieFullInfo ToMovieFullInfo(this Movie movie)
    {
        MovieFullInfo result = new MovieFullInfo();
        result.Id = movie.Id;
        result.Title = movie.Title;
        result.BackdropUrl = Path.Combine( movie.BaseFolderDir, movie.FolderName, movie.BackdropFilePath?.TrimStart('/') ?? string.Empty);
        result.Genres = movie.Genres.Select(a => a.Name).ToList();
        result.PosterUrl = Path.Combine( movie.BaseFolderDir, movie.FolderName, movie.PosterFilePath?.TrimStart('/') ?? string.Empty);   
        result.LocalPath = Path.Combine( movie.BaseFolderDir, movie.FolderName, movie.VideoFilePath ?? string.Empty);
        result.IMDBId = movie.IMDBId;
        result.Description = movie.Description;
        result.Year = movie.Year;
        result.CoverUrl = Path.Combine( movie.BaseFolderDir, movie.FolderName, movie.CoverFilePath ?? string.Empty);
        result.VoteAverage = movie.VoteAverage;
        result.VoteCount = movie.VoteCount;
        result.IsAdult = movie.IsAdult;
        return result;
    }
}
