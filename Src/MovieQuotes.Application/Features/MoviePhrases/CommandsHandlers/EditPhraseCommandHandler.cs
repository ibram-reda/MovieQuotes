namespace MovieQuotes.Application.Features.MoviePhrases.CommandsHandlers;

using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Common.Enums;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.MoviePhrases.Commands;
using MovieQuotes.Application.Features.MoviePhrases.Mappings;
using MovieQuotes.Application.Features.MoviePhrases.Models;
using MovieQuotes.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

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

        if (textEdit | durationEdited)
            await this.dbContext.SaveChangesAsync();

        if (durationEdited && !string.IsNullOrWhiteSpace(phrase.VideoClipPath))
        {
            phrase.DeleteVideoClip();
            await this.dbContext.SaveChangesAsync(cancellationToken);
        }

        result.Payload = phrase.ToPhrase();

        return result;
    }
}
