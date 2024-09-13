namespace MovieQuotes.Api.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using MovieQuotes.Api.Contracts.Responses;
using MovieQuotes.Application.Operations.Queries;

public class PhrasesController : BaseController
{ 

    [HttpGet("search")]
    public async Task<IActionResult> Get([FromQuery(Name = "q")] string SearchText, int resultPerPage = 10, int pageNumber = 0, CancellationToken token = default)
    {
        SearchForPhraseQuery query = new(SearchText)
        {
            ResultPerPage = resultPerPage,
            PageNumber = pageNumber,
        };
        var res = await this.mediator.Send(query, token);

        if (res.IsError)
            return HandelErrors(res.Errors);

        var phrases = this.mapper.Map<IEnumerable<PhraseResponse>>(res.Payload);


        return Ok(phrases);

    }
}
