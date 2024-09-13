namespace MovieQuotes.Application.Operations.Queries;

using MediatR;
using MovieQuotes.Application.Models;

public class SearchForPhraseQuery : IRequest<OperationResult<List<Phrase>>>
{
    public SearchForPhraseQuery(string searchText)
    {
        SearchText = searchText;
    }

    public string SearchText { get; }
    public int ResultPerPage { get; init; }
    public int PageNumber { get; init; }
}
