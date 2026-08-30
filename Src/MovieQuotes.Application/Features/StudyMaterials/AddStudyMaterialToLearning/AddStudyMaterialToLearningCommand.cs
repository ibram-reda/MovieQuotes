namespace MovieQuotes.Application.Features.StudyMaterials;
using MediatR;
using MovieQuotes.Application.Common.Models;

public class AddStudyMaterialToLearningCommand : IRequest<OperationResult<bool>>
{
    public AddStudyMaterialToLearningCommand(int studyMaterialId)
    {
        StudyMaterialId = studyMaterialId;
    }

    public int StudyMaterialId { get; }
}