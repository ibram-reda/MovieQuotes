namespace MovieQuotes.Application.Features.Movies.Queries;

using MediatR;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.Movies.Models;

public class GetMovieDetailsQuery : IRequest<OperationResult<MovieFullInfo>>
{
    public int MovieId { get; set; }
}
