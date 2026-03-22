namespace MovieQuotes.Application.Features.StudyPhrases.CommandsHandlers;

using MediatR;
using MovieQuotes.Application.Common.Enums;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.StudyPhrases.Commands;
using MovieQuotes.Application.Features.StudyPhrases.Mappings;
using MovieQuotes.Application.Features.StudyPhrases.Models;
using MovieQuotes.Infrastructure;
using System;
using System.Threading;
using System.Threading.Tasks;

internal class CreateStudyPhraseCommandHandler : IRequestHandler<CreateStudyPhraseCommand, OperationResult<StudyPhrase>>
{
    private readonly MovieQuotesDbContext dbContext;

    public CreateStudyPhraseCommandHandler(MovieQuotesDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<OperationResult<StudyPhrase>> Handle(CreateStudyPhraseCommand request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<StudyPhrase>();
        
        if(request is null)
            result.AddError(ErrorCode.InvalidInput, StudyPhraseMessages.InvalidData);
   
        if(request?.PhraseId <= 0)
            result.AddError(ErrorCode.InvalidInput,  StudyPhraseMessages.RequiredPhraseId);

        if (result.IsError)
            return result;

        try
        {
            var dbStudyPhrase = Domain.Models.StudyPhrase.CreateStudyPhrase(request!.PhraseId,
                request.StudyType,
                request.Content,
                request.Translation,
                request.ArContentTranslation,
                request.ArPhraseTranslation,
                request.Origin,
                request.Notes);

            dbContext.StudyPhrases.Add(dbStudyPhrase);
            await dbContext.SaveChangesAsync();

            var progress = Domain.Models.StudyPhraseProgress.Create(dbStudyPhrase.Id);
            dbContext.StudyPhraseProgress.Add(progress);
            await dbContext.SaveChangesAsync();

            result.Payload = dbStudyPhrase.ToStudyPhrase();

        }
        catch (Exception exception)
        {
            var ex = exception;
            while (ex is not null)
            {
                result.AddUnknownError(ex.Message);
                ex = ex.InnerException;
            }
        }

        return result;
    }
}
