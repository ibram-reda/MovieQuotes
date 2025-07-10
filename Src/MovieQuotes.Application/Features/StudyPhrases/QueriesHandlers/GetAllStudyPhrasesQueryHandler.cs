namespace MovieQuotes.Application.Features.StudyPhrases.QueriesHandlers;

using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.StudyPhrases.Models;
using MovieQuotes.Application.Features.StudyPhrases.Queries;
using MovieQuotes.Infrastructure;
using System.Threading;
using System.Threading.Tasks;
using static Constants;

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

        var qry = baseQuery.Select(a => new StudyPhrase
        {
            StudyId = a.Id,
            PhraseId = a.PhraseId,
            PhraseText = a.Phrase!.Text,
            MovieName = a.Phrase.Movie!.Title,
            StartTime = a.Phrase.StartTime,
            EndTime = a.Phrase.EndTime,
            VideoLocation = a.Phrase.VideoClipPath!.Replace(CashTemplate, CashPath),
            Content = a.Content,
            Translation = a.Translation,
            StudyType = a.StudyType
        });

        var totalCount = await qry.CountAsync();

        result.Payload = await qry.ToListAsync();
        result.Count = totalCount;
        result.CurrentPageNumber = 1;
        result.HasNext = false;

        return result;
    }
}
