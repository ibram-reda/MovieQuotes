namespace MovieQuotes.Application.Features.MoviePhrases.QueriesHandlers;

using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MovieQuotes.Application.Common.Enums;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.MoviePhrases.Models;
using MovieQuotes.Application.Features.MoviePhrases.Queries;
using MovieQuotes.Application.Features.StudyPhrases; 
using MovieQuotes.Infrastructure;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

public class SearchForPhraseQueryHandler : IRequestHandler<SearchForPhraseQuery, OperationPageResult<Phrase>>
{
    private readonly MovieQuotesDbContext dbContext;
    private readonly ILogger<SearchForPhraseQueryHandler> logger;

    public SearchForPhraseQueryHandler(MovieQuotesDbContext dbContext, ILogger<SearchForPhraseQueryHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }
    public async Task<OperationPageResult<Phrase>> Handle(SearchForPhraseQuery request, CancellationToken cancellationToken)
    {
        var result = new OperationPageResult<Phrase>();
         

        var query = dbContext.SubtitlePhrases
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
            });

        var totalCount = await query.CountAsync(cancellationToken);
        var itemCountToSkip = (int)(request.ResultPerPage * request.PageNumber);

        if (itemCountToSkip > totalCount)
        {
            result.AddError(ErrorCode.NotFound, StudyPhraseMessages.PageNotFound, request.PageNumber, totalCount / request.ResultPerPage);
            return result;
        }

        result.Payload = await query.Skip(itemCountToSkip)
            .Take((int)request.ResultPerPage)
            .ToListAsync(cancellationToken);

        result.Count = totalCount;
        result.CurrentPageNumber = request.PageNumber;
        result.ItemPerPage = request.ResultPerPage;
        result.HasNext = itemCountToSkip + result.ItemPerPage < totalCount;


        return result;

    }



}
