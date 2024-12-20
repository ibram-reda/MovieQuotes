namespace MovieQuotes.Application.Features.Movies.QueriesHandlers;

using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Features.Movies.Models;
using MovieQuotes.Application.Features.Movies.Queries;
using MovieQuotes.Application.Models;
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
            .Select(a => new MovieFullInfo
            {
                Id = a.Id,
                IMDBId = a.IMDBId,
                CoverUrl = a.CoverUrl,
                Description = a.Description,
                Title = a.Title,
                LocalPath = a.LocalPath,                
            });

        result.Payload = await query.FirstOrDefaultAsync(m => m.Id == request.MovieId);

        return result;
    }
}
