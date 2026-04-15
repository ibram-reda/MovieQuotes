namespace MovieQuotes.Infrastructure.Repositories;

using MovieQuotes.Domain.Interfaces;
using MovieQuotes.Domain.Models;

public class WordRepository : GenericRepository<Word>, IWordRepository
{
    public WordRepository(MovieQuotesDbContext context) : base(context)
    {
    }
}