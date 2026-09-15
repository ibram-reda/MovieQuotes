namespace MovieQuotes.Domain.Interfaces;

using MovieQuotes.Domain.Models;

public interface IStudyMaterialRepository : IGenericRepository<StudyMaterial>
{
    Task<List<StudyMaterial>> GetAllStudyMaterialsWithPhraseAsync(CancellationToken cancellationToken);
    Task<List<StudyMaterial>> GetDraftStudyMaterialsWithoutAITagsAsync(CancellationToken cancellationToken);
    Task<bool> RemovePhraseAsync(int studyMaterialId, int phraseId);
    
}