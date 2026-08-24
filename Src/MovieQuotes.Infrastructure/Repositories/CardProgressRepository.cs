namespace MovieQuotes.Infrastructure.Repositories;

using MovieQuotes.Domain.Interfaces;
using MovieQuotes.Domain.Models;

public class CardProgressRepository : GenericRepository<CardProgress>, ICardProgress
{
    public CardProgressRepository(MovieQuotesDbContext context) : base(context)
    {
    }
}
