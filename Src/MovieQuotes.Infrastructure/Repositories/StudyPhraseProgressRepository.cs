namespace MovieQuotes.Infrastructure.Repositories;

using MovieQuotes.Domain.Interfaces;
using MovieQuotes.Domain.Models;
using Microsoft.EntityFrameworkCore;

public class StudyPhraseProgressRepository : GenericRepository<StudyPhraseProgress>, IStudyPhraseProgressRepository
{
    public StudyPhraseProgressRepository(MovieQuotesDbContext context) : base(context)
    {
    }

    public async Task<StudyPhraseProgress?> GetProgressForPhrase(int studyPhraseId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.StudyPhraseId == studyPhraseId, cancellationToken);
    }

    
}