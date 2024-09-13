namespace MovieQuotes.Application.Operations.QueryHandlers;

using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Models;
using MovieQuotes.Application.Operations.Queries;
using MovieQuotes.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class SearchForPhraseQueryHandler : IRequestHandler<SearchForPhraseQuery, OperationResult<List<Phrase>>>
{
    private readonly MovieQuotesDbContext dbContext;
    public SearchForPhraseQueryHandler(MovieQuotesDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public async Task<OperationResult<List<Phrase>>> Handle(SearchForPhraseQuery request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<List<Phrase>>();
        var d = await dbContext.SubtitlePhrases.Include(a => a.Movie)
            .Where(a => a.Text.Contains(request.SearchText))
            .Select(a => new Phrase()
            {
                Id = a.Id,
                Sequence = a.Sequence,
                Text = a.Text,
                MovieName = a.Movie!.Title,
                MoviePath = a.Movie.LocalPath,
                VideoLocation = a.VideoClipPath!,
                Duration = a.Duration,
                StartTime = a.StartTime,
                EndTime = a.EndTime,
            })
            .Skip(request.ResultPerPage * request.PageNumber)
            .Take(request.ResultPerPage)
            .ToListAsync(cancellationToken);

        result.Payload = d;
        return result;

    }



}
