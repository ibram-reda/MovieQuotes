namespace MovieQuotes.Application.Features.Movies.Queries;

using MediatR;
using MovieQuotes.Application.Features.Movies.Models;
using MovieQuotes.Application.Models;


public class GetAllMoviesQuery : IRequest<OperationPageResult<MovieInfo>>
{
    /// <summary>
    /// get all movies in database.
    /// </summary>
    /// <param name="searchText">return all movies in database if it null</param>
    public GetAllMoviesQuery(string? searchText = null)
    {
        SearchText = searchText;
    }
    public string? SearchText { get; }
}
