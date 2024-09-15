namespace MovieQuotes.Application.Operations.QueryHandlers;

using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Enums;
using MovieQuotes.Application.Models;
using MovieQuotes.Application.Operations.Queries;
using MovieQuotes.Infrastructure;
using Xabe.FFmpeg;

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
            result.AddError(ErrorCode.NotFound, OperationsMessages.PhraseNotFound, request.PhraseId);
            return result;
        }

        phrase = await EnsureVideoExistAsync(phrase, cancellationToken);
        result.Payload = phrase.VideoLocation;

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
                                MoviePath = a.Movie.LocalPath,
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

    private async Task<Phrase> EnsureVideoExistAsync(Phrase phrase, CancellationToken token)
    {
        if (phrase.VideoLocation is not null)
            return phrase;

        string outputFileName = $"Cash/{phrase.MovieName}/{phrase.Sequence}.MP4";

        // if the video not in the cash folder generate it
        if (!File.Exists(outputFileName))
            await GenerateVideoAsync(phrase, outputFileName, token);

        // save result in database for the next time
        await this.dbContext.SubtitlePhrases
            .Where(a => a.Id == phrase.Id)
            .ExecuteUpdateAsync(a => a.SetProperty(k => k.VideoClipPath, outputFileName));

        phrase.VideoLocation = outputFileName;

        return phrase;
    }

    private static async Task GenerateVideoAsync(Phrase phrase, string outputFileName, CancellationToken token = default)
    {
        var mediaInfo = await FFmpeg.GetMediaInfo(phrase.MoviePath);
        var videoStream = mediaInfo.VideoStreams.First();
        var audioStream = mediaInfo.AudioStreams.First();

        var res = await FFmpeg.Conversions.New()
        .AddStream(videoStream.Split(phrase.StartTime, phrase.Duration))
        .AddStream(audioStream.Split(phrase.StartTime, phrase.Duration))
        .SetOutput(outputFileName)
        .Start(token);
    }
}
