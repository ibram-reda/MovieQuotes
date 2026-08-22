namespace MovieQuotes.Application.Features.Movies.QueriesHandlers;

using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Common.Enums;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.Movies.Models;
using MovieQuotes.Application.Features.Movies.Queries;
using MovieQuotes.Domain.Interfaces;

internal class GetMoviesWithStudyMaterialQueryHandler : IRequestHandler<GetMoviesWithStudyMaterialQuery, OperationPageResult<MovieWithStudyMaterialCount>>
{
    private readonly IMovieQUnitOfWork unitOfWork;

    public GetMoviesWithStudyMaterialQueryHandler(IMovieQUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<OperationPageResult<MovieWithStudyMaterialCount>> Handle(
        GetMoviesWithStudyMaterialQuery request,
        CancellationToken cancellationToken)
    {
        var result = new OperationPageResult<MovieWithStudyMaterialCount>();

        if (request is null)
        {
            result.AddError(ErrorCode.InvalidInput, "Movie query is required.");
            return result;
        }

        if (request.ItemsPerPage <= 0)
        {
            result.AddError(ErrorCode.InvalidInput, "Items per page must be greater than zero.");
            return result;
        }

        if (request.PageNumber <= 0)
        {
            result.AddError(ErrorCode.InvalidInput, "Page number must be greater than zero.");
            return result;
        }

        var groupedMovies = unitOfWork.StudyMaterials.Query
            .AsNoTracking()
            .Where(material => material.Phrase != null && material.Phrase.Movie != null)
            .GroupBy(material => new
            {
                MovieId = material.Phrase!.Movie!.Id,
                Title = material.Phrase.Movie.Title,
                Year = material.Phrase.Movie.Year,
                BaseFolderDir = material.Phrase.Movie.BaseFolderDir,
                FolderName = material.Phrase.Movie.FolderName,
                CoverUrl = material.Phrase.Movie.CoverFilePath
            })
            .Select(group => new MovieWithStudyMaterialCount
            {
                Id = group.Key.MovieId,
                Title = group.Key.Title,
                Year = group.Key.Year,
                CoverUrl = Path.Combine(group.Key.BaseFolderDir,group.Key.FolderName,group.Key.CoverUrl),
                StudyMaterialCount = group.Count()
            })
            .OrderByDescending(movie => movie.StudyMaterialCount)
            .ThenBy(movie => movie.StudyMaterialCount);

        var totalCount = await groupedMovies.CountAsync(cancellationToken);
        var itemsToSkip = checked((request.PageNumber - 1) * request.ItemsPerPage);
        var movies = await groupedMovies
            .Skip(itemsToSkip)
            .Take(request.ItemsPerPage)
            .ToListAsync(cancellationToken);

        result.Payload = movies;
        result.Count = totalCount;
        result.HasNext = itemsToSkip + movies.Count < totalCount;
        result.CurrentPageNumber = (uint)request.PageNumber;
        result.ItemPerPage = (uint)request.ItemsPerPage;

        return result;
    }
}
