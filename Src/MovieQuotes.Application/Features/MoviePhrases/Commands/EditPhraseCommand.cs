namespace MovieQuotes.Application.Features.MoviePhrases.Commands;

using MediatR;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.MoviePhrases.Models;

public class EditPhraseCommand : IRequest<OperationResult<Phrase>>
{
    public int PhraseId { get; set; }
    public int MovieId { get; set; }
    public int Sequence { get; set; }

    public string PhraseText { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }

}
