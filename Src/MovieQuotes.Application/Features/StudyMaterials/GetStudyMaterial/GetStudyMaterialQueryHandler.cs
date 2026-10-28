namespace MovieQuotes.Application.Features.StudyMaterials.GetStudyMaterial;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Common.Enums;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.StudyMaterials.AddNewStudyMaterial;
using MovieQuotes.Application.Features.StudyMaterials.AddPhraseToStudyMaterial;
using MovieQuotes.Application.Features.StudyMaterials.AddStudyMaterialToLearning;
using MovieQuotes.Application.Features.StudyMaterials.DeleteStudyMaterial;
using MovieQuotes.Application.Features.StudyMaterials.EditStudyMaterial;
using MovieQuotes.Application.Features.StudyMaterials.GetPartOfSpeechCounts;
using MovieQuotes.Application.Features.StudyMaterials.GetStudyMaterial;
using MovieQuotes.Application.Features.StudyMaterials.GetStudyMaterialLevels;
using MovieQuotes.Application.Features.StudyMaterials.GetStudyMaterials;
using MovieQuotes.Application.Features.StudyMaterials.Mappings;
using MovieQuotes.Application.Features.StudyMaterials.Models;
using MovieQuotes.Application.Features.StudyMaterials.UnlinkPhraseFromStudyMaterial;
using MovieQuotes.Application.Features.StudyMaterials;
using MovieQuotes.Domain.Interfaces;

public class GetStudyMaterialQueryHandler
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
            .Include(s=>s.StudyCards)
                .ThenInclude(card => card.Progresses)
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
            SrcPhrase = material.Phrase?.Text,
            ArPhraseTranslation = material.ArPhraseTranslation,
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
            IsCurrentlyLerning = material.StudyCards.Any(),
            Progresses = material.StudyCards.SelectMany(c=>c.Progresses.Select(p=>
                new StudyProgress
                {
                    ProgressType = p.ExerciseType.ToString(),
                    Repetitions = p.Repetitions,
                    ReviewCount = p.ReviewCount,
                    LapseCount = p.LapseCount,

                    NextReviewAt = p.NextReviewAt,
                    LastReviewedAt = p.LastReviewedAt,

                }
            )).ToList(),
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
