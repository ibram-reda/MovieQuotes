namespace MovieQuotes.Application.Features.Movies.QueriesHandlers;

using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.Movies.Mappings;
using MovieQuotes.Application.Features.Movies.Models;
using MovieQuotes.Application.Features.Movies.Queries;
using MovieQuotes.Domain.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


public class GetAllMoviesQueryHandler : IRequestHandler<GetAllMoviesQuery, OperationPageResult<MovieInfo>>
{
    private readonly IMovieQUnitOfWork unitOfWork;

    public GetAllMoviesQueryHandler(IMovieQUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }
    public async Task<OperationPageResult<MovieInfo>> Handle(GetAllMoviesQuery request, CancellationToken cancellationToken)
    {
        var result = new OperationPageResult<MovieInfo>();

        var query = unitOfWork.Movies.Query;

        var filters = MovieSearchParser.Parse(request.SearchText ?? "");

        if (!string.IsNullOrWhiteSpace(filters.Text))
        {
            query = query.Where(m =>
                m.Title.Contains(filters.Text) ||
                m.FolderName.Contains(filters.Text));
        }

        if (!string.IsNullOrWhiteSpace(filters.ImdbId))
        {
            query = query.Where(m => m.IMDBId == filters.ImdbId);
        }

        if (filters.Year.HasValue)
        {
            query = query.Where(m =>
                m.Year == filters.Year.Value);
        }

        if (filters.MinYear.HasValue)
        {
            query = query.Where(m =>
                m.Year >= filters.MinYear.Value);
        }

        if (filters.MaxYear.HasValue)
        {
            query = query.Where(m =>
                m.Year <= filters.MaxYear.Value);
        }

        if (!string.IsNullOrWhiteSpace(filters.Genre))
        {
            query = query.Where(m =>
                m.Genres.Any(g => g.Name == filters.Genre));
        }

        if (filters.MinRating.HasValue)
        {
            query = query.Where(m =>
                m.VoteAverage >= filters.MinRating.Value);
        }

        if (filters.MaxRating.HasValue)
        {
            query = query.Where(m =>
                m.VoteAverage <= filters.MaxRating.Value);
        }


        query = query.OrderByDescending(m => m.AddedDate);

        var PayLoad = await query
            .Select(a => a.ToMovieInfo())
            .Skip((request.PageNumber - 1) * request.ItemsPerPage)
            .Take(request.ItemsPerPage)
            .ToListAsync(cancellationToken);

        result.Count = await query.CountAsync();
        result.HasNext = result.Count > PayLoad.Count;
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
