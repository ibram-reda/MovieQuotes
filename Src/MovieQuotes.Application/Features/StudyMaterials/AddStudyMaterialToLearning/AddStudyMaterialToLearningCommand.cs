namespace MovieQuotes.Application.Features.StudyMaterials.AddStudyMaterialToLearning;
using MovieQuotes.Application.Common.Models;

public class AddStudyMaterialToLearningCommand
{
    public AddStudyMaterialToLearningCommand(int studyMaterialId)
    {
        StudyMaterialId = studyMaterialId;
    }

    public int StudyMaterialId { get; }
}
