namespace MovieQuotes.Application.Features.StudyMaterials;

using MediatR;
using MovieQuotes.Application.Common.Models;

public class DeleteStudyMaterialCommand : IRequest<OperationResult<bool>>
{
    public DeleteStudyMaterialCommand(int studyMaterialId)
    {
        StudyMaterialId = studyMaterialId;
    }

    public int StudyMaterialId { get; }
}