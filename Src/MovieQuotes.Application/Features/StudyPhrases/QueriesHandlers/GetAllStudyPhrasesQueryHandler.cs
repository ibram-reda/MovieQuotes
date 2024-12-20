namespace MovieQuotes.Application.Features.StudyPhrases.QueriesHandlers;

using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Features.StudyPhrases.Models;
using MovieQuotes.Application.Features.StudyPhrases.Queries;
using MovieQuotes.Application.Models;
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

        var qry = this.dbContext.StudyPhrases
            .Select(a => new StudyPhrase
            {
                PhraseId = a.PhraseId,
                PhraseText = a.Phrase!.Text,
                VideoLocation = a.Phrase.VideoClipPath!,
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
