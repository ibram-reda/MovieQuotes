namespace MovieQuotes.Api.Controllers;
 
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MovieQuotes.Api.Contracts;
using MovieQuotes.Application.Common.Models;

[ApiController]
[Route("[controller]")]
public abstract class BaseController :ControllerBase   
{
    private IMediator? _mediator; 
    protected IMediator mediator => this._mediator ??= HttpContext.RequestServices.GetService<IMediator>()!;
     
    protected IActionResult HandelErrors(IEnumerable<Error> errors)
    {
        var result = new BaseResponse<string>();
        result.IsSuccess = false;
        result.Errors = errors.Select(e => e.Message).ToList();

        return BadRequest(result);
    }
}
