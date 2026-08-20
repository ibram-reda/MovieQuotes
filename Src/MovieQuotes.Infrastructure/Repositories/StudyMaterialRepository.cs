namespace MovieQuotes.Infrastructure.Repositories;


using Microsoft.EntityFrameworkCore;
using MovieQuotes.Domain.Interfaces;
using MovieQuotes.Domain.Models;


public class StudyMaterialRepository : GenericRepository<StudyMaterial>, IStudyMaterialRepository
{
    public StudyMaterialRepository(MovieQuotesDbContext context) : base(context)
    {
    }

    public override Task<StudyMaterial?> GetByIdAsync(int id)
    {
        return _dbSet.Include(m => m.StudyCards).ThenInclude(c=>c.CardProgress).FirstOrDefaultAsync(m => m.Id == id);
    }
}