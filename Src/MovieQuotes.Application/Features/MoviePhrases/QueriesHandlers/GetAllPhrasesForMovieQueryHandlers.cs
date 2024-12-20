namespace MovieQuotes.Application.Features.MoviePhrases.QueriesHandlers;

using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Features.MoviePhrases.Models;
using MovieQuotes.Application.Features.MoviePhrases.Queries;
using MovieQuotes.Application.Models;
using MovieQuotes.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal class GetAllPhrasesForMovieQueryHandlers : IRequestHandler<GetAllPhrasesForMovieQuery, OperationResult<List<Phrase>>>
{
    private readonly MovieQuotesDbContext dbContext;

    public GetAllPhrasesForMovieQueryHandlers(MovieQuotesDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<OperationResult<List<Phrase>>> Handle(GetAllPhrasesForMovieQuery request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<List<Phrase>>();
        result.Payload = await this.dbContext.SubtitlePhrases
            .Where(a => a.MovieId == request.MovieId)
            .Select(a => new Phrase()
            {
                Id = a.Id,
                Sequence = a.Sequence,
                Text = a.Text,
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                Duration = a.Duration,
            })
            .ToListAsync();

        return result;
    }
}
