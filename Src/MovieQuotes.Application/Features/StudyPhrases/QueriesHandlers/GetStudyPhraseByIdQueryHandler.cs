namespace MovieQuotes.Application.Features.StudyPhrases.QueriesHandlers;

using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Common.Enums;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.StudyPhrases.Mappings;
using MovieQuotes.Application.Features.StudyPhrases.Models;
using MovieQuotes.Application.Features.StudyPhrases.Queries;
using MovieQuotes.Infrastructure;

internal class GetStudyPhraseByIdQueryHandler : IRequestHandler<GetStudyPhraseByIdQuery, OperationResult<StudyPhrase>>
{
    private readonly MovieQuotesDbContext dbContext;
    public GetStudyPhraseByIdQueryHandler(MovieQuotesDbContext db)
    {
        this.dbContext = db;
    }
    public async Task<OperationResult<StudyPhrase>> Handle(GetStudyPhraseByIdQuery request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<StudyPhrase>();

        var studyPhrase = await dbContext.StudyPhrases
            .FirstOrDefaultAsync(sp => sp.Id == request.StudyId, cancellationToken);
        if (studyPhrase == null)
        {
            result.AddError(ErrorCode.NotFound, StudyPhraseMessages.PhraseNotFound, request.StudyId);
            return result;
        }

        result.Payload = studyPhrase.ToStudyPhrase();

        return result;
    }
}
