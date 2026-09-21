namespace MovieQuotes.Application.Features.VideoClips.QueriesHandlers;

using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Common.Enums;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.MoviePhrases.Models;
using MovieQuotes.Application.Features.VideoClips.Commands;
using MovieQuotes.Application.Features.VideoClips.CommandsHandlers;
using MovieQuotes.Application.Features.VideoClips.Queries;
using MovieQuotes.Domain.Interfaces; 

internal class VideoClipQueryHandler(IMovieQUnitOfWork unitOfWork, AppSettings settings) : IRequestHandler<VideoClipQuery, OperationResult<string>>
{ 

    public async Task<OperationResult<string>> Handle(VideoClipQuery request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<string>();
        var phrase = unitOfWork.SubtitlePhrases.Query
            .Include(a => a.Movie)
            .Where(a => a.Id == request.PhraseId) 
            .FirstOrDefault();

        if (phrase is null)
        {
            result.AddError(ErrorCode.NotFound, VideoClipsMessages.PhraseNotFound, request.PhraseId);
            return result;
        }

        if (!string.IsNullOrEmpty(phrase.VideoClipPath) && File.Exists(phrase.VideoClipPath))
        {
            result.Payload = phrase.GetVideoClipPath(settings.VideoCashPath);
            return result;
        }
        
        await phrase.GenerateVideoClipAsync(settings.VideoCashPath); 
        // save result in database for the next time
        await unitOfWork.SubtitlePhrases.UpdateAsync(phrase);
        await unitOfWork.SaveAsync();
        result.Payload = phrase.GetVideoClipPath(settings.VideoCashPath);

        return result;
    }

}
