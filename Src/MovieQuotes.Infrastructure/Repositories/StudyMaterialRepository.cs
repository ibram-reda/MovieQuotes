namespace MovieQuotes.Infrastructure.Repositories;


using Microsoft.EntityFrameworkCore;
using MovieQuotes.Domain.Interfaces;
using MovieQuotes.Domain.Models;


public class StudyMaterialRepository : GenericRepository<StudyMaterial>, IStudyMaterialRepository
{
    public StudyMaterialRepository(MovieQuotesDbContext context) : base(context)
    {
    }

    public Task<List<StudyMaterial>> GetAllStudyMaterialsWithPhraseAsync(CancellationToken cancellationToken)
    {
        return _dbSet.Include(m => m.Phrase)
            .ToListAsync(cancellationToken);
    }

    public override Task<StudyMaterial?> GetByIdAsync(int id)
    {
        return _dbSet.Include(m => m.StudyCards)
        .ThenInclude(c=>c.Progresses).FirstOrDefaultAsync(m => m.Id == id);
    }

    

    public Task<List<StudyMaterial>> GetDraftStudyMaterialsWithoutAITagsAsync(CancellationToken cancellationToken)
    {
        return _dbSet.Where(m => m.IsDraft && m.Tags!.Contains("AI") == false)
            .Include(m => m.Phrase)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> RemovePhraseAsync(int studyMaterialId, int phraseId)
    {
        var materialPhrase = await _context.Set<StudyMaterialPhrase>()
            .SingleOrDefaultAsync(item => item.StudyMaterialId == studyMaterialId && item.PhraseId == phraseId);

        if (materialPhrase is null)
            return false;

        // prevent removing the first phrase in the study material, as it is considered the main phrase.
        if(materialPhrase.Sequance == 1)
            return false;

        _context.Set<StudyMaterialPhrase>().Remove(materialPhrase);
        return true;
    }
}