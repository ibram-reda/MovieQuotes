namespace MovieQuotes.Application.Operations.QueryHandlers;

using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Models;
using MovieQuotes.Application.Operations.Queries;
using MovieQuotes.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


public class GetAllMoviesQueryHandler : IRequestHandler<GetAllMoviesQuery, OperationPageResult<MovieInfo>>
{
    private readonly MovieQuotesDbContext dbContext;

    public GetAllMoviesQueryHandler(MovieQuotesDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public async Task<OperationPageResult<MovieInfo>> Handle(GetAllMoviesQuery request, CancellationToken cancellationToken)
    {
        var result = new OperationPageResult<MovieInfo>();

        var query =   this.dbContext.Movies
            .Select(a => new MovieInfo
            {
                Id = a.Id,
                IMDBId = a.IMDBId,
                CoverUrl = a.CoverUrl,
                Description = a.Description,
                Title = a.Title,
                LocalPath = a.LocalPath,
            });

        var PayLoad = await query.ToListAsync(cancellationToken);
        
        result.Count = PayLoad.Count;
        result.HasNext = false;
        result.Payload = PayLoad;
        result.CurrentPageNumber = 1;
        result.ItemPerPage = (uint)result.Count;

        return result;
    }
}


class GetAllMoviesQueryExceptionHandler : IRequestExceptionHandler<GetAllMoviesQuery, OperationPageResult<MovieInfo>, Exception>
{
    public Task Handle(GetAllMoviesQuery request, Exception exception, RequestExceptionHandlerState<OperationPageResult<MovieInfo>> state, CancellationToken cancellationToken)
    {
        var result = new OperationPageResult<MovieInfo>();
        var ex = exception;
        while (ex is not null)
        {
            result.AddUnknownError(ex.Message);
            ex = ex.InnerException;
        }
        state.SetHandled(result);
        return Task.CompletedTask;
    }
}
