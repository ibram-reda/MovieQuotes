namespace MovieQuotes.Application.Features.VideoClips.Commands;
using MovieQuotes.Application.Common.Models;


public class CreatePhraseClipCommand
{
    public CreatePhraseClipCommand(int phraseId)
    {
        PhraseId = phraseId; 
    }
    public int PhraseId { get; }
}