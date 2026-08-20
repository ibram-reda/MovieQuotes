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

internal class GetStudiesForActiveRecallQueryHandler : IRequestHandler<GetStudiesForActiveRecallQuery, OperationPageResult<StudyPhrase>>
{
    private readonly MovieQuotesDbContext dbContext;

    public GetStudiesForActiveRecallQueryHandler(MovieQuotesDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<OperationPageResult<StudyPhrase>> Handle(GetStudiesForActiveRecallQuery request, CancellationToken cancellationToken)
    {
        var result = new OperationPageResult<StudyPhrase>();

        var baseQuery = this.dbContext.StudyPhrases.AsQueryable();


       // should change to use only due phrases for active recall
        baseQuery = baseQuery.Where(a => a.Progress!.NextReviewDate <= DateTime.Now);
            

        var qry = baseQuery.Include(a => a.Phrase)
                           .Include(a => a.Phrase!.Movie)
                           .Include(a => a.Progress)
                           .OrderByDescending(a => a.AddedDate) 
                           .Select(a => a.ToStudyPhrase());

        var totalCount = await qry.CountAsync();

        result.Payload = await qry.ToListAsync();
        result.Count = totalCount;
        result.CurrentPageNumber = 1;
        result.HasNext = false;

        return result;
    }
}
