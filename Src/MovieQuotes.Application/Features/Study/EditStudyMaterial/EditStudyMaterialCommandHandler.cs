namespace MovieQuotes.Application.Features.Study.EditStudyMaterial;

using MediatR;
using MovieQuotes.Application.Common.Enums;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.Study;
using MovieQuotes.Domain.Interfaces;

internal class EditStudyMaterialCommandHandler : IRequestHandler<EditStudyMaterialCommand, OperationResult<StudyMaterial>>
{
    private readonly IMovieQUnitOfWork unitOfWork;

    public EditStudyMaterialCommandHandler(IMovieQUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<OperationResult<StudyMaterial>> Handle(EditStudyMaterialCommand request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<StudyMaterial>();

        if (request is null)
        {
            result.AddError(ErrorCode.InvalidInput, "Study material data is required.");
            return result;
        }

        if (request.StudyMaterialId <= 0)
        {
            result.AddError(ErrorCode.InvalidInput, "A valid study material id is required.");
            return result;
        }

        var studyMaterial = await unitOfWork.StudyMaterials.GetByIdAsync(request.StudyMaterialId);

        if (studyMaterial is null)
        {
            result.AddError(ErrorCode.NotFound, "Study material {0} was not found.", request.StudyMaterialId);
            return result;
        }

        studyMaterial.EditPartOfSpeach(request.PartOfSpeech);
        studyMaterial.EditContent(request.Content);
        studyMaterial.EditDefination(request.Definition);
        studyMaterial.EditContentArabicTranslation(request.ContentArabicTranslation);
        studyMaterial.EditArPhraseTranslation(request.ArPhraseTranslation);
        studyMaterial.EditOrigin(request.Origin);
        studyMaterial.EditNotes(request.Notes);
        studyMaterial.EditExamples(request.Examples);
        studyMaterial.EditSynonyms(request.Synonyms);
        studyMaterial.EditLevel(request.Level);
        studyMaterial.EditPronunciation(request.Pronunciation);
        studyMaterial.EditIsVulgar(request.IsVulgar);

        if (request.IsDraft)
            studyMaterial.MarkAsDraft();
        else
            studyMaterial.MarkAsReady();

        try
        {
            await unitOfWork.StudyMaterials.UpdateAsync(studyMaterial);
            var affectedRows = await unitOfWork.SaveAsync(cancellationToken);

            if (affectedRows <= 0)
            {
                result.AddError(ErrorCode.UpdateError, "Failed to update study material.");
                return result;
            }
        }
        catch (Exception exception)
        {
            result.AddException(exception);
            return result;
        }

        result.Payload = studyMaterial.ToStudyMaterial();
        return result;
    }
}