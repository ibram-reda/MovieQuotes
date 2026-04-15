namespace MovieQuotes.Infrastructure.Repositories;


using Microsoft.EntityFrameworkCore;
using MovieQuotes.Domain.Interfaces;
using MovieQuotes.Domain.Models;


public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly MovieQuotesDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(MovieQuotesDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public IQueryable<T> Query => _dbSet.AsQueryable();

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public virtual async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
    }

    public virtual async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null) return false;

        _dbSet.Remove(entity);
        return true;
    }
}
