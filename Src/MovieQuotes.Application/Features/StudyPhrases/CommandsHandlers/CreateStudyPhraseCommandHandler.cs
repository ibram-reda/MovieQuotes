namespace MovieQuotes.Application.Features.StudyPhrases.CommandsHandlers;

using MediatR;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.StudyPhrases.Commands;
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
        try
        {
            var dbStudyPhrase = Domain.Models.StudyPhrase.CreateStudyPhrase(request.PhraseId,
                request.StudyType,
                request.Content,
                request.Translation);

            dbContext.StudyPhrases.Add(dbStudyPhrase);
            var effectedRows = await dbContext.SaveChangesAsync();

            result.Payload = new StudyPhrase
            {
                Content = dbStudyPhrase.Content,
                Translation = dbStudyPhrase.Translation,
                StudyType = dbStudyPhrase.StudyType,               
            };

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
