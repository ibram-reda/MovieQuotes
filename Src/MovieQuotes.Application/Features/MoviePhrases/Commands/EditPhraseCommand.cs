namespace MovieQuotes.Application.Features.MoviePhrases.Commands;

using MediatR;
using MovieQuotes.Application.Common.Models;

public class EditPhraseCommand : IRequest<OperationResult<Unit>>
{
    public int MovieId { get; set; }
    public int Sequence { get; set; }

    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }

    public string Text { get; set; } = string.Empty;
}
