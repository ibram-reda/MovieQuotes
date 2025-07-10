namespace MovieQuotes.Application.Features.VideoClips.Commands;

using MediatR;
using MovieQuotes.Application.Common.Models;


internal class CreatePhraseClipCommand : IRequest<OperationResult<string>>
{
    public CreatePhraseClipCommand(int phraseId,string movieName, int sequence, string movieLocation, TimeSpan startTime, TimeSpan duration)
    {
        PhraseId = phraseId;
        MovieName = movieName;
        Sequence = sequence;
        MovieLocation = movieLocation;
        StartTime = startTime;
        Duration = duration;
    }
    public int PhraseId { get; }
    public string MovieName { get; }

    public int Sequence { get; }

    public string MovieLocation { get; }
    public TimeSpan StartTime { get; }
    public TimeSpan Duration { get; }

}