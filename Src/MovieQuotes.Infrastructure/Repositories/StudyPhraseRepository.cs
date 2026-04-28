namespace MovieQuotes.Infrastructure.Repositories;

using MovieQuotes.Domain.Interfaces;
using MovieQuotes.Domain.Models;
using Microsoft.EntityFrameworkCore;

public class StudyPhraseRepository : GenericRepository<StudyPhrase>, IStudyPhraseRepository
{
    public StudyPhraseRepository(MovieQuotesDbContext context) : base(context)
    {
    }

    override public async Task<StudyPhrase?> GetByIdAsync(int id)
    {
        return await _dbSet.Include(a => a.Phrase)
                    .Include(a=>a.Progress)
                    .FirstOrDefaultAsync(p => p.Id == id);
    }
}