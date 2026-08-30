namespace MovieQuotes.Application.Features.StudyMaterials;
using MediatR;
using MovieQuotes.Application.Common.Models;

public class AddPhraseToStudyMaterialCommand : IRequest<OperationResult<bool>>
{
    public AddPhraseToStudyMaterialCommand(int studyMaterialId, int phraseId)
    {
        StudyMaterialId = studyMaterialId;
        PhraseId = phraseId;
    }

    public int StudyMaterialId { get; }
    public int PhraseId { get; }
}