namespace MovieQuotes.Application.Features.Study.AddNewStudyMaterial;

using MediatR;
using MovieQuotes.Application.Common.Enums;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Domain.Interfaces;

internal class AddNewStudyMaterialCommandHandler : IRequestHandler<AddNewStudyMaterialCommand, OperationResult<StudyMaterial>>
{
    private readonly IMovieQUnitOfWork unitOfWork;

    public AddNewStudyMaterialCommandHandler(IMovieQUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<OperationResult<StudyMaterial>> Handle(AddNewStudyMaterialCommand request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<StudyMaterial>();

        if (request is null)
            result.AddError(ErrorCode.InvalidInput, "Study material data is required.");
        else if (request.PhraseId <= 0)
            result.AddError(ErrorCode.InvalidInput, "A valid phrase id is required.");

        if (result.IsError)
            return result;

        try
        {
            var studyMaterial = Domain.Models.StudyMaterial.CreateStudyMaterial(
                request!.PhraseId,
                request.PartOfSpeech,
                request.Content,
                request.Definition,
                request.ContentArabicTranslation,
                request.ArPhraseTranslation,
                request.Origin,
                request.Notes,
                request.IsDraft,
                request.Examples,
                request.Synonyms,
                request.Level,
                request.Pronunciation,
                request.IsVulgar);

            await unitOfWork.StudyMaterials.AddAsync(studyMaterial);
            await unitOfWork.SaveAsync(cancellationToken);

            result.Payload = studyMaterial.ToStudyMaterial();
        }
        catch (Exception exception)
        {
            result.AddException(exception);
        }

        return result;
    }
}