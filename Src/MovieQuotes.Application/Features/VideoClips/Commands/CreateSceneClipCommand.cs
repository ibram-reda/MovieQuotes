namespace MovieQuotes.Application.Features.VideoClips.Commands;

using MediatR;
using MovieQuotes.Application.Common.Models;

public class CreateSceneClipCommand : IRequest<OperationResult<string>>
{
    public CreateSceneClipCommand(string movieLocation,TimeSpan startTime,TimeSpan endTime)
    {
        MovieLocation = movieLocation;
        StartTime = startTime;
        EndTime = endTime;
    }

    public string MovieLocation { get; }
    public TimeSpan StartTime { get; }
    public TimeSpan EndTime { get; }
}
