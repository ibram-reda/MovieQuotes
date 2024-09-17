namespace MovieQuotes.Api.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using MovieQuotes.Api.Contracts;
using MovieQuotes.Api.Contracts.Responses;
using MovieQuotes.Application.Operations.Queries;

public class PhrasesController : BaseController
{ 

    [HttpGet("search")]
    public async Task<IActionResult> Get([FromQuery(Name = "q")] string SearchText, uint resultPerPage = 10, uint pageNumber = 0, CancellationToken token = default)
    {
        SearchForPhraseQuery query = new(SearchText)
        {
            ResultPerPage = resultPerPage,
            PageNumber = pageNumber,
        };
        var res = await this.mediator.Send(query, token);

        if (res.IsError)
            return HandelErrors(res.Errors);



        var phrases = this.mapper.Map<PagedResponse<PhraseResponse>>(res);


        return Ok(phrases);

    }
}
