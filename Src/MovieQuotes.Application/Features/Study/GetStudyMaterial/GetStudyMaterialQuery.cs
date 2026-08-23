namespace MovieQuotes.Application.Features.Study.GetStudyMaterial;

using MediatR;
using MovieQuotes.Application.Common.Models;

public class GetStudyMaterialQuery : IRequest<OperationResult<StudyMaterialDetails>>
{
    public GetStudyMaterialQuery(int studyMaterialId)
    {
        StudyMaterialId = studyMaterialId;
    }

    public int StudyMaterialId { get; }
}
