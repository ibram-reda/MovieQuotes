namespace MovieQuotes.Application.Features.StudyMaterials.GetStudyMaterial;
using MovieQuotes.Application.Common.Models;

public class GetStudyMaterialQuery
{
    public GetStudyMaterialQuery(int studyMaterialId)
    {
        StudyMaterialId = studyMaterialId;
    }

    public int StudyMaterialId { get; }
}
