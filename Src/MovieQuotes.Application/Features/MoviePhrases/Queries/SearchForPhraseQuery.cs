namespace MovieQuotes.Application.Features.MoviePhrases.Queries;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.MoviePhrases.Models;

public class SearchForPhraseQuery
{
    public SearchForPhraseQuery(string searchText)
    {
        SearchText = searchText;
    }

    public string SearchText { get; }

    public uint ResultPerPage { get; init; } = 10;
    public uint PageNumber { get; init; } = 0;
}
