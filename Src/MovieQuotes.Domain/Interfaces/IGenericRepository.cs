namespace MovieQuotes.Domain.Interfaces;

using System.Linq;

public interface IGenericRepository<T> where T : class
{
    IQueryable<T> Query { get; }
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task<bool> DeleteAsync(int id);
}
