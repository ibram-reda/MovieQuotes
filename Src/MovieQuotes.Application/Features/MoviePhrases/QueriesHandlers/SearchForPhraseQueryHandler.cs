namespace MovieQuotes.Application.Features.MoviePhrases.QueriesHandlers;

using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MovieQuotes.Application.Common.Enums;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.MoviePhrases.Mappings;
using MovieQuotes.Application.Features.MoviePhrases.Models;
using MovieQuotes.Application.Features.MoviePhrases.Queries;
using MovieQuotes.Domain.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class SearchForPhraseQueryHandler : IRequestHandler<SearchForPhraseQuery, OperationPageResult<Phrase>>
{
    private readonly IMovieQUnitOfWork unitOfWork;
    private readonly ILogger<SearchForPhraseQueryHandler> logger;

    public SearchForPhraseQueryHandler(IMovieQUnitOfWork unitOfWork, ILogger<SearchForPhraseQueryHandler> logger)
    {
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }
    public async Task<OperationPageResult<Phrase>> Handle(SearchForPhraseQuery request, CancellationToken cancellationToken)
    {
        var result = new OperationPageResult<Phrase>();

        if (string.IsNullOrWhiteSpace(request.SearchText))
        {
            result.AddError(ErrorCode.InvalidInput, MoviePhrasesMessages.SearchTextEmpty);
            return result;
        }

        var query = unitOfWork.SubtitlePhrases.Query
            .Where(a => a.Text.Contains(request.SearchText))
            .Include(a => a.Movie)
            .Select(a => a.ToPhrase());

        var totalCount = await query.CountAsync(cancellationToken);
        var itemCountToSkip = (int)(request.ResultPerPage * request.PageNumber);

        if (itemCountToSkip > totalCount)
        {
            result.AddError(ErrorCode.NotFound, MoviePhrasesMessages.PageNotFound, request.PageNumber, totalCount / request.ResultPerPage);
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
