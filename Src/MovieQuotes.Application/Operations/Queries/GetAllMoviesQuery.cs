namespace MovieQuotes.Application.Operations.Queries;

using MediatR;
using MovieQuotes.Application.Models;


public class GetAllMoviesQuery : IRequest<OperationPageResult<MovieInfo>>
{
}
