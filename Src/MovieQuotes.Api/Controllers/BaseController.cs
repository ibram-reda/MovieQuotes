namespace MovieQuotes.Api.Controllers;

using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MovieQuotes.Api.Contracts;
using MovieQuotes.Application.Models;

[ApiController]
[Route("[controller]")]
public abstract class BaseController :ControllerBase   
{
    private IMediator? _mediator;
    private IMapper? _mapper;
    protected IMediator mediator => this._mediator ??= HttpContext.RequestServices.GetService<IMediator>()!;
    protected IMapper mapper => this._mapper ??= HttpContext.RequestServices.GetService<IMapper>()!;


    protected IActionResult HandelErrors(IEnumerable<Error> errors)
    {
        var result = new BaseResponse<string>();
        result.IsSuccess = false;
        result.Errors = errors.Select(e => e.Message).ToList();

        return BadRequest(result);
    }
}
