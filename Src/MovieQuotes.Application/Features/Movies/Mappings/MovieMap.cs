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
        result.LocalPath = movie.LocalPath;
        result.IMDBId = movie.IMDBId;
        result.Description = movie.Description;
        result.CoverUrl = movie.CoverUrl;
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
        result.LocalPath = movie.LocalPath;
        result.IMDBId = movie.IMDBId;
        result.Description = movie.Description;
        result.Year = movie.Year;
        result.CoverUrl = movie.CoverUrl;
        //result.BaseFolderDir = movie.BaseFolderDir;
        //result.FolderName = movie.FolderName;
        //result.AddedDate = movie.AddedDate;
        return result;
    }
}
