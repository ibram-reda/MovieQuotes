namespace MovieQuotes.Domain.Interfaces;

using MovieQuotes.Domain.Models;

public interface IStudyPhraseProgressRepository : IGenericRepository<StudyPhraseProgress>
{
    
    Task<StudyPhraseProgress?> GetProgressForPhrase(int studyPhraseId, CancellationToken cancellationToken = default);
}