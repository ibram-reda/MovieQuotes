namespace MovieQuotes.Application.Features.VideoClips.Queries;
using MovieQuotes.Application.Common.Models;

public class VideoClipQuery
{
    public int PhraseId { get; }
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
