namespace MovieQuotes.Domain.Interfaces;

using MovieQuotes.Domain.Models;

public interface IStudyMaterialRepository : IGenericRepository<StudyMaterial>
{
	Task<bool> RemovePhraseAsync(int studyMaterialId, int phraseId);
    
}