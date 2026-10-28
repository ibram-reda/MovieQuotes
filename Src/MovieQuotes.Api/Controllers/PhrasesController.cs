namespace MovieQuotes.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using MovieQuotes.Api.Contracts;
using MovieQuotes.Api.Contracts.Responses;
using MovieQuotes.Application.Features.MoviePhrases.Queries;
using MovieQuotes.Application.Features.MoviePhrases.QueriesHandlers;

public class PhrasesController : BaseController
{
    private readonly SearchForPhraseQueryHandler searchForPhraseHandler;

    public PhrasesController(SearchForPhraseQueryHandler searchForPhraseHandler)
    {
        this.searchForPhraseHandler = searchForPhraseHandler;
    }

    [HttpGet("search")]
    public async Task<IActionResult> Get([FromQuery(Name = "q")] string SearchText, uint resultPerPage = 10, uint pageNumber = 0, CancellationToken token = default)
    {
        SearchForPhraseQuery query = new(SearchText)
        {
            ResultPerPage = resultPerPage,
            PageNumber = pageNumber,
        };
        var res = await this.searchForPhraseHandler.Handle(query, token);

        if (res.IsError)
            return HandelErrors(res.Errors);



        var phrases = res;


        return Ok(phrases);

    }
}
