namespace MovieQuotes.Application.Features.Movies.QueriesHandlers;

using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Features.Movies.Models;
using MovieQuotes.Application.Features.Movies.Queries;
using MovieQuotes.Application.Models;
using MovieQuotes.Infrastructure;

internal class GetAllMoviesWithoutSubtitlesQueryHandler : IRequestHandler<GetAllMoviesWithoutSubtitlesQuery, OperationPageResult<MovieInfo>>
{
    private readonly MovieQuotesDbContext dbContext;

    public GetAllMoviesWithoutSubtitlesQueryHandler(MovieQuotesDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<OperationPageResult<MovieInfo>> Handle(GetAllMoviesWithoutSubtitlesQuery request, CancellationToken cancellationToken)
    {
        var result = new OperationPageResult<MovieInfo>();

        var query = this.dbContext.Movies
             .Where(m => !m.Subtitles.Any())
             .Select(a => new MovieInfo
             {
                 Id = a.Id,
                 BaseFolderDir = a.BaseFolderDir,
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
