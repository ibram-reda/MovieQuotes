namespace MovieQuotes.Infrastructure.Repositories;

using MovieQuotes.Domain.Interfaces;
using MovieQuotes.Domain.Models;

public class PhraseWordsRepository : GenericRepository<PhraseWords>, IPhraseWordsRepository
{
    public PhraseWordsRepository(MovieQuotesDbContext context) : base(context)
    {
    }
}