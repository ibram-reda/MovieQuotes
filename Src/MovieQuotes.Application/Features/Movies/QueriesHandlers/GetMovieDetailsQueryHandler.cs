namespace MovieQuotes.Application.Features.Movies.QueriesHandlers;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.Movies.Mappings;
using MovieQuotes.Application.Features.Movies.Models;
using MovieQuotes.Application.Features.Movies.Queries;
using MovieQuotes.Domain.Interfaces;

public class GetMovieDetailsQueryHandler
{
    private readonly IMovieQUnitOfWork unitOfWork;
    public GetMovieDetailsQueryHandler(IMovieQUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }
    public async Task<OperationResult<MovieFullInfo>> Handle(GetMovieDetailsQuery request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<MovieFullInfo>();

        var query = unitOfWork.Movies.Query
            .Where(a => a.Id == request.MovieId)
            .Include(a => a.Genres)
            .Select(a => a.ToMovieFullInfo());

        result.Payload = await query.AsNoTracking().FirstOrDefaultAsync();

        return result;
    }
}
