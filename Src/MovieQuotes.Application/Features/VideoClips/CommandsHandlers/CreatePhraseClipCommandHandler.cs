namespace MovieQuotes.Application.Features.VideoClips.CommandsHandlers;

using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Common.Enums;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.VideoClips.Commands;
using MovieQuotes.Domain.Interfaces;
using System.Diagnostics; 

internal class CreatePhraseClipCommandHandler : IRequestHandler<CreatePhraseClipCommand, OperationResult<string>>
{
    private readonly IMovieQUnitOfWork unitOfWork;
    private readonly AppSettings settings;

    public CreatePhraseClipCommandHandler(IMovieQUnitOfWork unitOfWork,AppSettings settings)
    {
        this.unitOfWork = unitOfWork;
        this.settings = settings;
    }

    public async Task<OperationResult<string>> Handle(CreatePhraseClipCommand request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<string>();
        var phrase = await unitOfWork.SubtitlePhrases.Query
            .Include(a => a.Movie)
            .Where(a => a.Id == request.PhraseId)
            .FirstOrDefaultAsync(cancellationToken);

        if (phrase is null)
        {
            result.AddError(ErrorCode.NotFound, VideoClipsMessages.PhraseNotFound, request.PhraseId);
            return result;
        }

        if (!string.IsNullOrEmpty(phrase.VideoClipPath) && File.Exists(phrase.VideoClipPath))
        {
            result.Payload = phrase.VideoClipPath;
            return result;
        }

        await phrase.GenerateVideoClipAsync(settings.VideoCashPath);
        // save result in database for the next time
        await unitOfWork.SubtitlePhrases.UpdateAsync(phrase);
        await unitOfWork.SaveAsync();

        return result;
    }
}
