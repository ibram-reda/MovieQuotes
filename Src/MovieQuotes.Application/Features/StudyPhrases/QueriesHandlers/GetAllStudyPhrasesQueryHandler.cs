namespace MovieQuotes.Application.Features.StudyPhrases.QueriesHandlers;

using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.StudyPhrases.Mappings;
using MovieQuotes.Application.Features.StudyPhrases.Models;
using MovieQuotes.Application.Features.StudyPhrases.Queries;
using MovieQuotes.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

internal class GetAllStudyPhrasesQueryHandler : IRequestHandler<GetAllStudyPhrasesQuery, OperationPageResult<StudyPhrase>>
{
    private readonly MovieQuotesDbContext dbContext;

    public GetAllStudyPhrasesQueryHandler(MovieQuotesDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<OperationPageResult<StudyPhrase>> Handle(GetAllStudyPhrasesQuery request, CancellationToken cancellationToken)
    {
        var result = new OperationPageResult<StudyPhrase>();

        var baseQuery = this.dbContext.StudyPhrases.AsQueryable();

        if (request.MovieId > 0)
            baseQuery = baseQuery.Where(a => a.Phrase!.MovieId == request.MovieId);

        var qry = baseQuery.Include(a=>a.Phrase)
                           .Include(a => a.Phrase!.Movie)
                           .Select(a => a.ToStudyPhrase());

        var totalCount = await qry.CountAsync();

        result.Payload = await qry.ToListAsync();
        result.Count = totalCount;
        result.CurrentPageNumber = 1;
        result.HasNext = false;

        return result;
    }
}
