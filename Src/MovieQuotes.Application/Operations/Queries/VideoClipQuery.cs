namespace MovieQuotes.Application.Operations.Queries;

using MediatR;
using MovieQuotes.Application.Models;

public class VideoClipQuery : IRequest<OperationResult<string>>
{
    public int PhraseId { get;  }
    public string? MovieName { get; }
    public int Sequence { get; } 

    public bool CanUseId => PhraseId > 0;

    public VideoClipQuery(int phraseId)
    {
        PhraseId = phraseId;
    }

    public VideoClipQuery(string movieName, int sequence)
    {
        MovieName = movieName;
        Sequence = sequence;
    }
}
