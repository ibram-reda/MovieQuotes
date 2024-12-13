namespace MovieQuotes.Application.Operations.QueryHandlers;

using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Models;
using MovieQuotes.Application.Operations.Queries;
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

        var query = this.dbContext.Movies
            .Select(a => new MovieFullInfo
            {
                Id = a.Id,
                IMDBId = a.IMDBId,
                CoverUrl = a.CoverUrl,
                Description = a.Description,
                Title = a.Title,
                LocalPath = a.LocalPath,
                phrases = a.Subtitles.Select(f => new Phrase
                {
                    Id = f.Id,
                    Duration = f.Duration,
                    StartTime = f.StartTime,
                    EndTime = f.EndTime,
                    Text = f.Text,
                    Sequence = f.Sequence,
                }).ToList()
            });

        result.Payload = await query.FirstOrDefaultAsync(m=>m.Id ==request.MovieId);

        return result;
    }
}
