namespace MovieQuotes.Application.Features.Study.GetStudyMaterial;

using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Common.Enums;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.Study;
using MovieQuotes.Domain.Interfaces;

internal class GetStudyMaterialQueryHandler : IRequestHandler<GetStudyMaterialQuery, OperationResult<StudyMaterialDetails>>
{
    private readonly IMovieQUnitOfWork unitOfWork;

    public GetStudyMaterialQueryHandler(IMovieQUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<OperationResult<StudyMaterialDetails>> Handle(
        GetStudyMaterialQuery request,
        CancellationToken cancellationToken)
    {
        var result = new OperationResult<StudyMaterialDetails>();

        if (request is null || request.StudyMaterialId <= 0)
        {
            result.AddError(ErrorCode.InvalidInput, "A valid study material id is required.");
            return result;
        }

        var material = await unitOfWork.StudyMaterials.Query
            .AsNoTracking()
            .Include(studyMaterial => studyMaterial.Phrases)
                .ThenInclude(materialPhrase => materialPhrase.Phrase)
                    .ThenInclude(phrase => phrase.Movie)
            .SingleOrDefaultAsync(
                studyMaterial => studyMaterial.Id == request.StudyMaterialId,
                cancellationToken);

        if (material is null)
        {
            result.AddError(ErrorCode.NotFound, "Study material {0} was not found.", request.StudyMaterialId);
            return result;
        }

        result.Payload = new StudyMaterialDetails
        {
            Id = material.Id,
            PartOfSpeech = material.PartOfSpeech,
            Content = material.Content,
            ContentArabicTranslation = material.ContentArabicTranslation,
            Origin = material.Origin,
            Definition = material.Definition,
            IsDraft = material.IsDraft,
            Examples = material.Examples,
            Synonyms = material.Synonyms,
            Level = material.Level,
            Pronunciation = material.Pronunciation,
            IsVulgar = material.IsVulgar,
            Notes = material.Notes,
            Tags = material.Tags,
            MaterialPhrases = material.Phrases
                .OrderBy(materialPhrase => materialPhrase.Sequance)
                .Select(materialPhrase => new StudyMaterialPhrase
                {
                    PhraseId = materialPhrase.PhraseId,
                    PhraseText = materialPhrase.Phrase.Text,
                    ArPhraseTranslation = materialPhrase.ArabicTranslation ?? string.Empty,
                    MovieCoverUrl = materialPhrase.Phrase.Movie?.GetCoverUrl()??"",
                    MovieTitle = materialPhrase.Phrase.Movie?.Title ?? string.Empty,
                    Sequance = materialPhrase.Sequance
                })
                .ToList()
        };

        return result;
    }
}
