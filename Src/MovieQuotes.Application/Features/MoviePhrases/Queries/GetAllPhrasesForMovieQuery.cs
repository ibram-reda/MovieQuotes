namespace MovieQuotes.Application.Features.MoviePhrases.Queries;

using MediatR;
using MovieQuotes.Application.Features.MoviePhrases.Models;
using MovieQuotes.Application.Models;


public class GetAllPhrasesForMovieQuery : IRequest<OperationResult<List<Phrase>>>
{
    public int MovieId { get; set; }
}
