namespace MovieQuotes.Application.Features.StudyMaterials;
using MediatR;
using MovieQuotes.Application.Common.Enums;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Domain.Interfaces;

internal class UnlinkPhraseFromStudyMaterialCommandHandler : IRequestHandler<UnlinkPhraseFromStudyMaterialCommand, OperationResult<bool>>
{
    private readonly IMovieQUnitOfWork unitOfWork;

    public UnlinkPhraseFromStudyMaterialCommandHandler(IMovieQUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<OperationResult<bool>> Handle(
        UnlinkPhraseFromStudyMaterialCommand request,
        CancellationToken cancellationToken)
    {
        var result = new OperationResult<bool>();
        if (request is null || request.StudyMaterialId <= 0 || request.PhraseId <= 0)
        {
            result.AddError(ErrorCode.InvalidInput, "Valid study material and phrase ids are required.");
            return result;
        }

        try
        {
            var removed = await unitOfWork.StudyMaterials.RemovePhraseAsync(request.StudyMaterialId, request.PhraseId);
            if (!removed)
            {

                result.AddError(ErrorCode.UpdateError, "Failed to unlink the phrase from the study material.");
                result.AddError(ErrorCode.UpdateError, "Either the phrase does not exist in the study material or it is the main phrase #1 and cannot be removed.");
                return result;
            }

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