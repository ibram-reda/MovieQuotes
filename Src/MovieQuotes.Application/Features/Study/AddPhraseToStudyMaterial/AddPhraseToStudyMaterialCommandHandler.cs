namespace MovieQuotes.Application.Features.Study.AddPhraseToStudyMaterial;

using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Common.Enums;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Domain.Interfaces;

internal class AddPhraseToStudyMaterialCommandHandler : IRequestHandler<AddPhraseToStudyMaterialCommand, OperationResult<bool>>
{
    private readonly IMovieQUnitOfWork unitOfWork;

    public AddPhraseToStudyMaterialCommandHandler(IMovieQUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<OperationResult<bool>> Handle(AddPhraseToStudyMaterialCommand request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<bool>();
        if (request is null || request.StudyMaterialId <= 0 || request.PhraseId <= 0)
        {
            result.AddError(ErrorCode.InvalidInput, "Valid study material and phrase ids are required.");
            return result;
        }

        var studyMaterial = await unitOfWork.StudyMaterials.Query
            .Include(material => material.Phrases)
            .SingleOrDefaultAsync(material => material.Id == request.StudyMaterialId, cancellationToken);
        if (studyMaterial is null)
        {
            result.AddError(ErrorCode.NotFound, "Study material {0} was not found.", request.StudyMaterialId);
            return result;
        }

        var phraseExists = await unitOfWork.SubtitlePhrases.Query
            .AnyAsync(phrase => phrase.Id == request.PhraseId, cancellationToken);
        if (!phraseExists)
        {
            result.AddError(ErrorCode.NotFound, "Phrase {0} was not found.", request.PhraseId);
            return result;
        }

        if (studyMaterial.Phrases.Any(materialPhrase => materialPhrase.PhraseId == request.PhraseId))
        {
            result.AddError(ErrorCode.InvalidInput, "Phrase {0} is already related to this study material.", request.PhraseId);
            return result;
        }

        try
        {
            studyMaterial.AddPhrase(request.PhraseId);
            await unitOfWork.SaveAsync(cancellationToken);
            result.Payload = true;
        }
        catch (Exception exception)
        {
            result.AddException(exception);
        }

        return result;
    }
}