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

using static Constants;

internal class EditPhraseCommandHandler : IRequestHandler<EditPhraseCommand, OperationResult<Phrase>>
{
    private readonly MovieQuotesDbContext dbContext;

    public EditPhraseCommandHandler(MovieQuotesDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public async Task<OperationResult<Phrase>> Handle(EditPhraseCommand request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<Phrase>();

        var phrase = await this.dbContext.SubtitlePhrases
            .FirstOrDefaultAsync(a => a.Id == request.PhraseId || (a.MovieId == request.MovieId && a.Sequence == request.Sequence));

        if (phrase is null)
        {
            result.AddError(ErrorCode.NotFound, "Phrase not found");
            return result;
        }

        var durationEdited = phrase.EditDuration(request.StartTime, request.EndTime);
        var textEdit = phrase.EditText(request.PhraseText);

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

        result.Payload = new Phrase
        {
            Id = phrase.Id, 
            Sequence = phrase.Sequence,
            StartTime = phrase.StartTime,
            EndTime = phrase.EndTime,
            Text = phrase.Text,
            VideoLocation = phrase.VideoClipPath!.Replace(CashTemplate, CashPath),
        };

        return result;
    }
}
