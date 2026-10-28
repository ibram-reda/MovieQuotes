namespace MovieQuotes.Api.Controllers;
 
using Microsoft.AspNetCore.Mvc;
using MovieQuotes.Api.Contracts;
using MovieQuotes.Application.Common.Models;

[ApiController]
[Route("[controller]")]
public abstract class BaseController :ControllerBase   
{
    protected IActionResult HandelErrors(IEnumerable<Error> errors)
    {
        var result = new BaseResponse<string>();
        result.IsSuccess = false;
        result.Errors = errors.Select(e => e.Message).ToList();

        return BadRequest(result);
    }
}
