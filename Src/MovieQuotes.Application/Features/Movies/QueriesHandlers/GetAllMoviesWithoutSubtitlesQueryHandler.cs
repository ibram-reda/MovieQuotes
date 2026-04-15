namespace MovieQuotes.Application.Features.Movies.QueriesHandlers;

using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.Movies.Mappings;
using MovieQuotes.Application.Features.Movies.Models;
using MovieQuotes.Application.Features.Movies.Queries;
using MovieQuotes.Domain.Interfaces;

internal class GetAllMoviesWithoutSubtitlesQueryHandler : IRequestHandler<GetAllMoviesWithoutSubtitlesQuery, OperationPageResult<MovieInfo>>
{
    private readonly IMovieQUnitOfWork unitOfWork;

    public GetAllMoviesWithoutSubtitlesQueryHandler(IMovieQUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<OperationPageResult<MovieInfo>> Handle(GetAllMoviesWithoutSubtitlesQuery request, CancellationToken cancellationToken)
    {
        var result = new OperationPageResult<MovieInfo>();

        var query = unitOfWork.Movies.Query
             .Where(m => !m.Subtitles.Any())
             .Select(a => a.ToMovieInfo());

        var PayLoad = await query.ToListAsync(cancellationToken);

        result.Count = PayLoad.Count;
        result.HasNext = false;
        result.Payload = PayLoad;
        result.CurrentPageNumber = 1;
        result.ItemPerPage = (uint)result.Count;

        return result;
    }
}
