namespace MovieQuotes.Application.Features.Movies.Queries;

using MediatR;
using MovieQuotes.Application.Common.Models;

public class IsMovieExistQuery : IRequest<OperationResult<bool>>
{
    /// <summary>
    /// folder name of the movie on `movie title (xxxx)` where xxxx is Release year.
    /// </summary>
    public string FolderName { get; set; } = string.Empty;
}
