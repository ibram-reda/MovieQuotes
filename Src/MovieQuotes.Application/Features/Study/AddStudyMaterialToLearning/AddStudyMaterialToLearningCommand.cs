namespace MovieQuotes.Application.Features.Study.AddStudyMaterialToLearning;

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