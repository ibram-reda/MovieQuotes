namespace MovieQuotes.Application.Features.Movies.Queries;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.Movies.Models;


public class GetAllMoviesQuery
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

    public int ItemsPerPage = 100;
    public int PageNumber = 1;


}
