namespace MovieQuotes.Application.Features.VideoClips.CommandsHandlers;

using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Features.VideoClips.Commands;
using MovieQuotes.Application.Models;
using MovieQuotes.Infrastructure;
using Xabe.FFmpeg;

internal class CreateVideoClipCommandHandler : IRequestHandler<CreateVideoClipCommand, OperationResult<string>>
{
    private readonly MovieQuotesDbContext dbContext;
    private const string CashPath = @"D:\Cash";
    private const string CashTemplate = "@Cash";

    public CreateVideoClipCommandHandler(MovieQuotesDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<OperationResult<string>> Handle(CreateVideoClipCommand request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<string>();
        string outputFileName = $"{CashTemplate}/{request.MovieName}/{request.Sequence}.MP4";

        // if the video not in the cash folder generate it
        if (!File.Exists(outputFileName.Replace(CashTemplate, CashPath)))
            await GenerateVideoAsync(request.MovieLocation, request.StartTime, request.Duration, outputFileName, cancellationToken);

        await SaveInDataBase(request.PhraseId, outputFileName);

        result.Payload = outputFileName.Replace(CashTemplate, CashPath);
        return result;
    }

    private async Task GenerateVideoAsync(string moviePath, TimeSpan startTime, TimeSpan duration, string outputFileName, CancellationToken token = default)
    {
        var mediaInfo = await FFmpeg.GetMediaInfo(moviePath);
        var videoStream = mediaInfo.VideoStreams.First();
        var audioStream = mediaInfo.AudioStreams.First();

        var sTime = startTime;
        var dTime = duration;

        // if duration is less than 3sec add 1sec to the phrase
        if (duration < TimeSpan.FromSeconds(3))
        {
            sTime = startTime.Subtract(TimeSpan.FromSeconds(0.5));
            dTime = duration.Add(TimeSpan.FromSeconds(1));
        }

        var res = await FFmpeg.Conversions.New() 
        .AddStream(videoStream.Split(sTime, dTime))
        .AddStream(audioStream.Split(sTime, dTime))
        .SetOutput(outputFileName.Replace(CashTemplate, CashPath))
        .Start(token);
    }

    private async Task SaveInDataBase(int phraseId, string phraseClipLocation)
    {
        // save result in database for the next time
        await dbContext.SubtitlePhrases
            .Where(a => a.Id == phraseId)
            .ExecuteUpdateAsync(a => a.SetProperty(k => k.VideoClipPath, phraseClipLocation));
    }
}
