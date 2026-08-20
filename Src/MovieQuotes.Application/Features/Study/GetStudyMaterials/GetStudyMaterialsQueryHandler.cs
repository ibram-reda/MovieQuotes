namespace MovieQuotes.Application.Features.Study.GetStudyMaterials;

using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Common.Enums;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.Study;
using MovieQuotes.Domain.Interfaces;

internal class GetStudyMaterialsQueryHandler : IRequestHandler<GetStudyMaterialsQuery, OperationPageResult<StudyMaterial>>
{
    private readonly IMovieQUnitOfWork unitOfWork;

    public GetStudyMaterialsQueryHandler(IMovieQUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<OperationPageResult<StudyMaterial>> Handle(GetStudyMaterialsQuery request, CancellationToken cancellationToken)
    {
        var result = new OperationPageResult<StudyMaterial>();

        if (request is null)
        {
            result.AddError(ErrorCode.InvalidInput, "Study material query is required.");
            return result;
        }

        if (request.ResultPerPage == 0)
        {
            result.AddError(ErrorCode.InvalidInput, "Result per page must be greater than zero.");
            return result;
        }

        var query = unitOfWork.StudyMaterials.Query.AsNoTracking();

        if (request.MovieId > 0)
            query = query.Where(material => material.Phrase!.MovieId == request.MovieId);

        if (!string.IsNullOrWhiteSpace(request.Level))
            query = query.Where(material => material.Level == request.Level);

        if (!string.IsNullOrWhiteSpace(request.PartOfSpeech))
            query = query.Where(material => material.PartOfSpeech == request.PartOfSpeech);

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var searchText = request.SearchText.Trim();
            query = query.Where(material =>
                (material.Content != null && material.Content.Contains(searchText)) ||
                (material.Definition != null && material.Definition.Contains(searchText)) ||
                (material.ContentArabicTranslation != null && material.ContentArabicTranslation.Contains(searchText)) ||
                (material.ArPhraseTranslation != null && material.ArPhraseTranslation.Contains(searchText)) ||
                (material.Origin != null && material.Origin.Contains(searchText)) ||
                (material.Notes != null && material.Notes.Contains(searchText)) ||
                material.Examples.Contains(searchText) ||
                material.Synonyms.Contains(searchText));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var itemCountToSkip = checked((int)(request.ResultPerPage * request.PageNumber));

        if (itemCountToSkip >= totalCount && totalCount > 0)
        {
            result.AddError(ErrorCode.NotFound, "Study material page was not found.");
            return result;
        }

        var materials = await query
            .OrderByDescending(material => material.ModifiedDate)
            .ThenByDescending(material => material.Id)
            .Skip(itemCountToSkip)
            .Take((int)request.ResultPerPage)
            .ToListAsync(cancellationToken);

        result.Payload = materials.Select(material => material.ToStudyMaterial()).ToList();
        result.Count = totalCount;
        result.CurrentPageNumber = request.PageNumber;
        result.ItemPerPage = request.ResultPerPage;
        result.HasNext = itemCountToSkip + materials.Count < totalCount;

        return result;
    }
}