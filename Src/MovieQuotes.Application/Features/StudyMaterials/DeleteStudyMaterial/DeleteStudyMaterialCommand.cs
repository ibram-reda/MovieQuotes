namespace MovieQuotes.Application.Features.StudyMaterials.DeleteStudyMaterial;
using MovieQuotes.Application.Common.Models;

public class DeleteStudyMaterialCommand
{
    public DeleteStudyMaterialCommand(int studyMaterialId)
    {
        StudyMaterialId = studyMaterialId;
    }

    public int StudyMaterialId { get; }
}
