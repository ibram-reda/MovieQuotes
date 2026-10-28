namespace MovieQuotes.Application.Features.StudyMaterials.AddPhraseToStudyMaterial;
using MovieQuotes.Application.Common.Models;

public class AddPhraseToStudyMaterialCommand
{
    public AddPhraseToStudyMaterialCommand(int studyMaterialId, int phraseId)
    {
        StudyMaterialId = studyMaterialId;
        PhraseId = phraseId;
    }

    public int StudyMaterialId { get; }
    public int PhraseId { get; }
}
