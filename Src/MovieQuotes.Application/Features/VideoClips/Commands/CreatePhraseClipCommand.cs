namespace MovieQuotes.Application.Features.VideoClips.Commands;

using MediatR;
using MovieQuotes.Application.Common.Models;


internal class CreatePhraseClipCommand : IRequest<OperationResult<string>>
{
    public CreatePhraseClipCommand(int phraseId)
    {
        PhraseId = phraseId; 
    }
    public int PhraseId { get; }
}