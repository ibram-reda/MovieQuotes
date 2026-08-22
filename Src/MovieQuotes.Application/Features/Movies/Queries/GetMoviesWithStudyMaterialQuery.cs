namespace MovieQuotes.Application.Features.Movies.Queries;

using MediatR;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.Movies.Models;


public class GetMoviesWithStudyMaterialQuery : IRequest<OperationPageResult<MovieWithStudyMaterialCount>>
{
    /// <summary>
    /// get all movies in database that Has Study Meterial with it.
    /// </summary>
    /// <param name="searchText">return all movies in database that Has study material with it</param>
    public GetMoviesWithStudyMaterialQuery()
    { 
    } 

    public int ItemsPerPage = 100;
    public int PageNumber = 1;


}