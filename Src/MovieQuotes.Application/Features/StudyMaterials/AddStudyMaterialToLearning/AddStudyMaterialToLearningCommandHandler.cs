namespace MovieQuotes.Application.Features.StudyMaterials;
using MediatR;
using MovieQuotes.Application.Common.Enums;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Domain.Interfaces;
using MovieQuotes.Domain.Models;

internal class AddStudyMaterialToLearningCommandHandler : IRequestHandler<AddStudyMaterialToLearningCommand, OperationResult<bool>>
{
    private readonly IMovieQUnitOfWork unitOfWork;

    public AddStudyMaterialToLearningCommandHandler(IMovieQUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<OperationResult<bool>> Handle(AddStudyMaterialToLearningCommand request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<bool>();
        if (request is null || request.StudyMaterialId <= 0)
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

        if (studyMaterial.StudyCards.Count > 0)
        {
            result.AddError(ErrorCode.InvalidInput, "Study material {0} is already in learning.", request.StudyMaterialId);
            return result;
        }

           var trans =  await unitOfWork.BeginTransactionAsync();
        try
        {
            var card = StudyCard.Create(studyMaterial.Id);
            await unitOfWork.StudyCards.AddAsync(card);
            await unitOfWork.SaveAsync(cancellationToken);

            var recognitionProgress = CardProgress.Create(card.Id,StudyExerciseType.Recognition);
            var recallProgress = CardProgress.Create(card.Id,StudyExerciseType.ContextRecall);

            await unitOfWork.CardProgress.AddAsync(recallProgress);
            await unitOfWork.CardProgress.AddAsync(recognitionProgress);
            await unitOfWork.SaveAsync(cancellationToken);
            await unitOfWork.CommitTransactionAsync();
            result.Payload = true;
        }
        catch (Exception exception)
        {
            await unitOfWork.RollbackTransactionAsync();
            result.AddException(exception);
        }

        return result;
    }
}