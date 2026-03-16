namespace MovieQuotes.Application.Features.VideoClips.QueriesHandlers;

using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Common.Enums;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.MoviePhrases.Models;
using MovieQuotes.Application.Features.VideoClips.Commands;
using MovieQuotes.Application.Features.VideoClips.CommandsHandlers;
using MovieQuotes.Application.Features.VideoClips.Queries;
using MovieQuotes.Infrastructure;

internal class VideoClipQueryHandler : IRequestHandler<VideoClipQuery, OperationResult<string>>
{
    private readonly MovieQuotesDbContext dbContext;
    public VideoClipQueryHandler(MovieQuotesDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<OperationResult<string>> Handle(VideoClipQuery request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<string>();
        Phrase? phrase = await GetPhraseFromDBAsync(request, cancellationToken);

        if (phrase is null)
        {
            result.AddError(ErrorCode.NotFound, VideoClipsMessages.PhraseNotFound, request.PhraseId);
            return result;
        }

        var cmd = new CreatePhraseClipCommand(phrase.Id,phrase.MovieName,phrase.Sequence,phrase.MoviePath,phrase.StartTime,phrase.Duration);
        var handler = new CreatePhraseClipCommandHandler(dbContext);
        var rst = await handler.Handle(cmd, cancellationToken);
        if (rst.IsError)
            result.AddErrorRange(rst.Errors);
        else
            result.Payload = rst.Payload;

        return result;
    }

    private async Task<Phrase?> GetPhraseFromDBAsync(VideoClipQuery request, CancellationToken token)
    {
        var dbQuery = dbContext.SubtitlePhrases
                            .Select(a => new Phrase()
                            {
                                Id = a.Id,
                                Sequence = a.Sequence,
                                MovieName = a.Movie!.Title,
                                MoviePath = Path.Combine( a.Movie.BaseFolderDir, a.Movie.FolderName, a.Movie.VideoFilePath ?? string.Empty),
                                VideoLocation = a.VideoClipPath!,
                                Duration = a.Duration,
                                StartTime = a.StartTime,
                                EndTime = a.EndTime,
                            });

        dbQuery = request.CanUseId switch
        {
            true => dbQuery.Where(a => a.Id == request.PhraseId),
            false => dbQuery.Where(a => a.MovieName == request.MovieName && a.Sequence == request.Sequence),
        };

        return await dbQuery.FirstOrDefaultAsync(token);
    }
}
