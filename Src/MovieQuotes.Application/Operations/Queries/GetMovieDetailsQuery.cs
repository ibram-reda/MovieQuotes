namespace MovieQuotes.Application.Operations.Queries;

using MediatR;
using MovieQuotes.Application.Models;

public class GetMovieDetailsQuery : IRequest<OperationResult<MovieFullInfo>>
{
    public int MovieId { get; set; }
}
