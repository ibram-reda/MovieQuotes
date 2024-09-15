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


public class GetAllMoviesQueryHandler : IRequestHandler<GetAllMoviesQuery, OperationResult<List<MovieInfo>>>
{
    private readonly MovieQuotesDbContext dbContext;

    public GetAllMoviesQueryHandler(MovieQuotesDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public async Task<OperationResult<List<MovieInfo>>> Handle(GetAllMoviesQuery request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<List<MovieInfo>>();

        result.Payload = await this.dbContext.Movies
            .Select(a => new MovieInfo
            {
                Id = a.Id,
                IMDBId = a.IMDBId,
                CoverUrl = a.CoverUrl,
                Description = a.Description,
                Title = a.Title,
                LocalPath = a.LocalPath,
            }).ToListAsync(cancellationToken);

        return result;
    }
}


class GetAllMoviesQueryExceptionHandler : IRequestExceptionHandler<GetAllMoviesQuery, OperationResult<List<MovieInfo>>, Exception>
{
    public Task Handle(GetAllMoviesQuery request, Exception exception, RequestExceptionHandlerState<OperationResult<List<MovieInfo>>> state, CancellationToken cancellationToken)
    {
        var result = new OperationResult<List<MovieInfo>>();
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
