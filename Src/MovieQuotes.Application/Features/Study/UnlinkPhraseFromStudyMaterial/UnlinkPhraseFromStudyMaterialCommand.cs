namespace MovieQuotes.Application.Features.Study.UnlinkPhraseFromStudyMaterial;

using MediatR;
using MovieQuotes.Application.Common.Models;

public class UnlinkPhraseFromStudyMaterialCommand : IRequest<OperationResult<bool>>
{
    public UnlinkPhraseFromStudyMaterialCommand(int studyMaterialId, int phraseId)
    {
        StudyMaterialId = studyMaterialId;
        PhraseId = phraseId;
    }

    public int StudyMaterialId { get; }
    public int PhraseId { get; }
}