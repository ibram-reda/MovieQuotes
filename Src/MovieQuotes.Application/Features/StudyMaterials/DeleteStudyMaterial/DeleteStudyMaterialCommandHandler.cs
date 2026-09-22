namespace MovieQuotes.Application.Features.StudyMaterials;

using MediatR;
using MovieQuotes.Application.Common.Enums;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Domain.Interfaces;

internal class DeleteStudyMaterialCommandHandler : IRequestHandler<DeleteStudyMaterialCommand, OperationResult<bool>>
{
    private readonly IMovieQUnitOfWork unitOfWork;

    public DeleteStudyMaterialCommandHandler(IMovieQUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<OperationResult<bool>> Handle(
        DeleteStudyMaterialCommand request,
        CancellationToken cancellationToken)
    {
        var result = new OperationResult<bool>();

        if (request is null || request.StudyMaterialId <= 0)
        {
            result.AddError(ErrorCode.InvalidInput, "A valid study material id is required.");
            return result;
        }

        try
        {
            var deleted = await unitOfWork.StudyMaterials.DeleteAsync(request.StudyMaterialId);
            if (!deleted)
            {
                result.AddError(ErrorCode.NotFound, "Study material {0} was not found.", request.StudyMaterialId);
                return result;
            }

            var affectedRows = await unitOfWork.SaveAsync(cancellationToken);
            if (affectedRows <= 0)
            {
                result.AddError(ErrorCode.UpdateError, "Failed to delete study material {0}.", request.StudyMaterialId);
                return result;
            }

            result.Payload = true;
        }
        catch (Exception exception)
        {
            result.AddException(exception);
        }

        return result;
    }
}