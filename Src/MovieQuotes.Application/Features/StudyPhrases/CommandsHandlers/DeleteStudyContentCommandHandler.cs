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
using Microsoft.EntityFrameworkCore;

internal class DeleteStudyContentCommandHandler : IRequestHandler<DeleteStudyContentCommand, OperationResult<int>>
{
    private readonly MovieQuotesDbContext dbContext;

    public DeleteStudyContentCommandHandler(MovieQuotesDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<OperationResult<int>> Handle(DeleteStudyContentCommand request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<int>();
        
        if(request is null)
            result.AddError(ErrorCode.InvalidInput, StudyPhraseMessages.InvalidData);
   
        if(request.StudyContentId <= 0)
            result.AddError(ErrorCode.InvalidInput,  StudyPhraseMessages.RequiredStudyId);

        if (result.IsError)
            return result;

        try
        {
            var dbStudyPhrase = await dbContext.StudyPhrases.FirstOrDefaultAsync(s=>s.Id == request.StudyContentId);

            if (!string.IsNullOrEmpty(dbStudyPhrase.Content))
            {
                result.AddError(ErrorCode.InvalidInput,StudyPhraseMessages.ContentShouldBeEmptyToDelete);
                return result;
            }

            dbContext.StudyPhrases.Remove(dbStudyPhrase);
            await dbContext.SaveChangesAsync();             

            result.Payload = dbStudyPhrase.Id;

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
