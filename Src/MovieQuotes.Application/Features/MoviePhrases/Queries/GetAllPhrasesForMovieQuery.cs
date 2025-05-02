namespace MovieQuotes.Application.Features.MoviePhrases.Queries;

using MediatR;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.MoviePhrases.Models;

public enum Language
{
    ar,
    en
};

public class GetAllPhrasesForMovieQuery : IRequest<OperationResult<List<Phrase>>>
{
    public int MovieId { get; set; }

    public Language Language { get; set; } = Language.en;
}
