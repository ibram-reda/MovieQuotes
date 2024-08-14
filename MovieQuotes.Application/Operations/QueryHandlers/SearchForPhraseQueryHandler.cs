namespace MovieQuotes.Application.Operations.QueryHandlers;

using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Models;
using MovieQuotes.Application.Operations.Queries;
using MovieQuotes.Domain.Models;
using MovieQuotes.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


public class SearchForPhraseQueryHandler : IRequestHandler<SearchForPhraseQuery, OperationResult<List<SubtitlePhrase>>>
{
    private readonly MovieQuotesDbContext dbContext;
    public SearchForPhraseQueryHandler(MovieQuotesDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public async Task<OperationResult<List<SubtitlePhrase>>> Handle(SearchForPhraseQuery request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<List<SubtitlePhrase>>();

        try
        {

            var d = await dbContext.SubtitlePhrases.Include(a => a.Movie)
                .Where(a => a.Text.Contains(request.SearchText))
                .ToListAsync();

            result.Payload = d;
        }
        catch (Exception ex)
        {
            result.AddUnknownError(ex.Message);
        }


        return result;
    }
}
