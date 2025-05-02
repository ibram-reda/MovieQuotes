namespace MovieQuotes.Application.Features.MoviePhrases.CommandsHandlers;

using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Common.Enums;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.MoviePhrases.Commands;
using MovieQuotes.Application.Features.MoviePhrases.Models;
using MovieQuotes.Infrastructure;
using System;
using System.Threading;
using System.Threading.Tasks;


internal class EditPhraseCommandHandler : IRequestHandler<EditPhraseCommand, OperationResult<Unit>>
{
    private readonly MovieQuotesDbContext dbContext;

    public EditPhraseCommandHandler(MovieQuotesDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public async Task<OperationResult<Unit>> Handle(EditPhraseCommand request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<Unit>();

        var phrase = await this.dbContext.SubtitlePhrases
            .FirstOrDefaultAsync(a => a.MovieId == request.MovieId && a.Sequence == request.Sequence);

        if (phrase is null)
        {
            result.AddError(ErrorCode.NotFound, "Phrase not found");
            return result;
        }

        var durationEdited = phrase.EditDuration(request.StartTime, request.EndTime);
        var textEdit = phrase.EditText(request.Text);

        if(textEdit | durationEdited) 
            await this.dbContext.SaveChangesAsync();
        
        if(durationEdited && !string.IsNullOrWhiteSpace(phrase.VideoClipPath))
        {
            var actualPath = phrase.VideoClipPath.Replace(Constants.CashTemplate, Constants.CashPath);
            File.Delete(actualPath);
             
            await dbContext.SubtitlePhrases
                .Where(a => a.Id == phrase.Id)
                .ExecuteUpdateAsync(a => a.SetProperty(k => k.VideoClipPath, (string?)null));
        }


        return result;
    }
}
