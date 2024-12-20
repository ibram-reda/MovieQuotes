namespace MovieQuotes.Application.Features.Movies.Queries;

using MediatR;
using MovieQuotes.Application.Features.Movies.Models;
using MovieQuotes.Application.Models;

public class GetMovieDetailsQuery : IRequest<OperationResult<MovieFullInfo>>
{
    public int MovieId { get; set; }
}
