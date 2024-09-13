namespace MovieQuotes.Application.Operations.QueryHandlers;

using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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

        var phrase = await dbQuery.FirstOrDefaultAsync();
                     

        if (phrase is null)
        {
            result.AddError(ErrorCode.NotFound, OperationsMessages.PhraseNotFound, request.PhraseId);
            return result;
        }

        phrase = await ProcessAsync(phrase);
        result.Payload = phrase.VideoLocation;

        return result;
    }

    private async Task<Phrase> ProcessAsync(Phrase phrase)
    {
        if (phrase.VideoLocation is not null)
            return phrase;

        var mediaInfo = await FFmpeg.GetMediaInfo(phrase.MoviePath);
        var videoStream = mediaInfo.VideoStreams.First();
        var audioStream = mediaInfo.AudioStreams.First();

        string outputFileName = $"Cash/{phrase.MovieName}/{phrase.Sequence}.MP4";

        if (!File.Exists(outputFileName))
        {
            var res = await FFmpeg.Conversions.New()
            .AddStream(videoStream.Split(phrase.StartTime, phrase.Duration))
            .AddStream(audioStream.Split(phrase.StartTime, phrase.Duration))
            .SetOutput(outputFileName)
            .Start(); 
        } 

        // save result in database
        await this.dbContext.SubtitlePhrases
            .Where(a => a.Id == phrase.Id)
            .ExecuteUpdateAsync(a => a.SetProperty(k => k.VideoClipPath, outputFileName));

        phrase.VideoLocation = outputFileName;

        return phrase;

    }
}
