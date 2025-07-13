namespace MovieQuotes.Application.Features.Movies.QueriesHandlers;

using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.Movies.Mappings;
using MovieQuotes.Application.Features.Movies.Models;
using MovieQuotes.Application.Features.Movies.Queries;
using MovieQuotes.Infrastructure;

internal class GetMovieDetailsQueryHandler : IRequestHandler<GetMovieDetailsQuery, OperationResult<MovieFullInfo>>
{
    private readonly MovieQuotesDbContext dbContext;
    public GetMovieDetailsQueryHandler(MovieQuotesDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public async Task<OperationResult<MovieFullInfo>> Handle(GetMovieDetailsQuery request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<MovieFullInfo>();

        var query = dbContext.Movies
            .Where(a => a.Id == request.MovieId)
            .Select(a => a.ToMovieFullInfo());

        result.Payload = await query.FirstOrDefaultAsync();

        return result;
    }
}
