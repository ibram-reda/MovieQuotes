using MediatR;
using MovieQuotes.Application.Common.Enums;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Common.Services;
using MovieQuotes.Application.Features.SubtitleFiles.Commands;

namespace MovieQuotes.Application.Features.SubtitleFiles.CommandsHandlers;

internal class SubtitleResyncCommandHandler : IRequestHandler<SubtitleResyncCommand, OperationResult<Unit>>
{
    public async Task<OperationResult<Unit>> Handle(SubtitleResyncCommand request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<Unit>();
        if (string.IsNullOrEmpty(request.SubtitleFilePath))
            result.AddError(ErrorCode.ValidationError, SubtitleFilesErrorMessages.ArgumentNullOrEmpty, nameof(request.SubtitleFilePath));

        if (string.IsNullOrEmpty(request.OutPutFileName))
            result.AddError(ErrorCode.ValidationError, SubtitleFilesErrorMessages.ArgumentNullOrEmpty, nameof(request.OutPutFileName));

        // load subtitle
        var loadResult = await SubtitleManager.LoadAsync(request.SubtitleFilePath,request.InputFileEncoding);
        if (loadResult.IsError)
            result.AddErrorRange(loadResult.Errors);

        if (loadResult.Payload is null)
            result.AddUnknownError("can not load subtitles");

        // in memory shift
        loadResult.Payload?.Shift(request.TimeShift);

        // write to desk
        var outputDir = Path.GetDirectoryName(request.SubtitleFilePath) ?? "";
        var outputFilePath = Path.Combine(outputDir, request.OutPutFileName);
        var writeResult = await loadResult.Payload!.WriteToDeskAsync(outputFilePath);
        if (writeResult.IsError)
            result.AddErrorRange(writeResult.Errors);

        return result;
    }
}
