namespace MovieQuotes.Application.Features.MoviePhrases.CommandsHandlers;

using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Common.Enums;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.MoviePhrases.Commands;
using MovieQuotes.Application.Features.MoviePhrases.Mappings;
using MovieQuotes.Application.Features.MoviePhrases.Models;
using MovieQuotes.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

internal class EditPhraseCommandHandler : IRequestHandler<EditPhraseCommand, OperationResult<Phrase>>
{
    private readonly IMovieQUnitOfWork unitOfWork;

    public EditPhraseCommandHandler(IMovieQUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }
    public async Task<OperationResult<Phrase>> Handle(EditPhraseCommand request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<Phrase>();

        var phrase = await unitOfWork.SubtitlePhrases.Query
            .FirstOrDefaultAsync(a => a.Id == request.PhraseId || (a.MovieId == request.MovieId && a.Sequence == request.Sequence));

        if (phrase is null)
        {
            result.AddError(ErrorCode.NotFound, MoviePhrasesMessages.PhraseNotFound,request.PhraseId);
            return result;
        }

        var durationEdited = phrase.EditDuration(request.StartTime, request.EndTime);
        var textEdit = phrase.EditText(request.PhraseText);

        if (textEdit | durationEdited)
        {
            await unitOfWork.SubtitlePhrases.UpdateAsync(phrase);
            await unitOfWork.SaveAsync();
        }

        if (durationEdited && !string.IsNullOrWhiteSpace(phrase.VideoClipPath))
        {
            phrase.DeleteVideoClip(); // Delete the existing video clip if the duration has changed 
            await unitOfWork.SubtitlePhrases.UpdateAsync(phrase);
            await unitOfWork.SaveAsync();
        }

        result.Payload = phrase.ToPhrase();

        return result;
    }
}
