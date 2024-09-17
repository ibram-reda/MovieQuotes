namespace MovieQuotes.Application.Operations.Queries;

using MediatR;
using MovieQuotes.Application.Models;

public class SearchForPhraseQuery : IRequest<OperationPageResult<Phrase>>
{
    public SearchForPhraseQuery(string searchText)
    {
        SearchText = searchText;
    }

    public string SearchText { get; }

    public uint ResultPerPage { get; init; }
    public uint PageNumber { get; init; }
}
