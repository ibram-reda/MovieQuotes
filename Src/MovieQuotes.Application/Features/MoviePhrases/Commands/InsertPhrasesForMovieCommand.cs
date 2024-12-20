namespace MovieQuotes.Application.Features.MoviePhrases.Commands;

using MediatR;
using MovieQuotes.Application.Models;

public class InsertPhrasesForMovieCommand : IRequest<OperationResult<bool>>
{
    public long MovieId { get; set; }
    public string SubtitleLocation { get; set; } = "";
     
}
