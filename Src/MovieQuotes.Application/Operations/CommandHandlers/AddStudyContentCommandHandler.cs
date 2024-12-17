namespace MovieQuotes.Application.Operations.CommandHandlers;

using MediatR;
using MovieQuotes.Application.Models;
using MovieQuotes.Application.Operations.Commands;
using MovieQuotes.Domain.Models;
using MovieQuotes.Infrastructure;
using System;
using System.Threading;
using System.Threading.Tasks;

internal class AddStudyContentCommandHandler : IRequestHandler<AddStudyContentCommand, OperationResult<bool>>
{
    private readonly MovieQuotesDbContext dbContext;

    public AddStudyContentCommandHandler(MovieQuotesDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<OperationResult<bool>> Handle(AddStudyContentCommand request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<bool>();
        try
        {
            var studyPhrase = StudyPhrase.CreateStudyPhrase(request.PhraseId,
                request.StudyType,
                request.Content,
                request.Translation);

            dbContext.StudyPhrases.Add(studyPhrase);
            var effectedRows = await dbContext.SaveChangesAsync();
            result.Payload = effectedRows > 0;
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
