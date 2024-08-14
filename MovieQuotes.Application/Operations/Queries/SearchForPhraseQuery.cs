namespace MovieQuotes.Application.Operations.Queries;

using MediatR;
using MovieQuotes.Application.Models;
using MovieQuotes.Domain.Models;


public class SearchForPhraseQuery : IRequest<OperationResult<List<SubtitlePhrase>>>
{
    public SearchForPhraseQuery(string searchText)
    {
        SearchText = searchText;
    }

    public string SearchText { get; }
}
