namespace MovieQuotes.Application.Features.VideoClips.CommandsHandlers;

using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Common.Enums;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.VideoClips.Commands;
using MovieQuotes.Domain.Interfaces;
using System.Diagnostics;
using static Constants;

internal class CreatePhraseClipCommandHandler : IRequestHandler<CreatePhraseClipCommand, OperationResult<string>>
{
    private readonly IMovieQUnitOfWork unitOfWork;
    

    public CreatePhraseClipCommandHandler(IMovieQUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<OperationResult<string>> Handle(CreatePhraseClipCommand request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<string>();
        string outputFileName = $"{CashTemplate}/{request.MovieName}/{request.Sequence}.MP4";

        var clipLocation = outputFileName.Replace(CashTemplate, CashPath);
        // if the video not in the cash folder generate it
        if (!File.Exists(clipLocation))
        {
            var vedioResult = await GenerateVideoAsync(request.MovieLocation, request.StartTime, request.Duration, clipLocation, cancellationToken);
            if (vedioResult.IsError)
                result.AddErrorRange(vedioResult.Errors);
        }

        if (result.IsSuccess)
        {
            result.Payload = clipLocation;
            await SaveInDataBase(request.PhraseId, outputFileName);
        }

        return result;
    }

    private void EnsureDirectoryExist(string path)
    {
        var directory = Path.GetDirectoryName(path);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory!);
        }
    }
    private async Task<OperationResult<Unit>> GenerateVideoAsync(string moviePath, TimeSpan startTime, TimeSpan duration, string outputLocation, CancellationToken token = default)
    {
        var result = new OperationResult<Unit>();
        // add 200ms small time tolerance to the phrase
        var sTime = startTime.Subtract(TimeSpan.FromMilliseconds(100));
        var dTime = duration.Add(TimeSpan.FromMilliseconds(200)); ;

        EnsureDirectoryExist(outputLocation);
        var startInfo = new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = $"-ss {sTime} -y -i \"{moviePath}\" -t {dTime} \"{outputLocation}\"",
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardError = true,
        };

        using var process = new Process { StartInfo = startInfo };
        process.Start();

        // from documents  
        // To avoid deadlocks, always read the output stream first and then wait.  
        var error = await process.StandardError.ReadToEndAsync();
        if (!process.WaitForExit(60000)) // 1 min timeout
        {
            process.Kill();
            result.AddError(ErrorCode.TimeOutError, VideoClipsMessages.GenerateVideoTimeOut);
        }
        if (error.Contains("Error"))
            result.AddError(ErrorCode.FFMPEGError, error);

        return result;
    }
    private async Task SaveInDataBase(int phraseId, string phraseClipLocation)
    {
        // save result in database for the next time
        await unitOfWork.SubtitlePhrases.Query
            .Where(a => a.Id == phraseId)
            .ExecuteUpdateAsync(a => a.SetProperty(k => k.VideoClipPath, phraseClipLocation));
    }
}
