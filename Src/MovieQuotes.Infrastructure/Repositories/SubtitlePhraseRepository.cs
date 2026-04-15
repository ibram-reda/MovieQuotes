namespace MovieQuotes.Infrastructure.Repositories;

using MovieQuotes.Domain.Interfaces;
using MovieQuotes.Domain.Models;

public class SubtitlePhraseRepository : GenericRepository<SubtitlePhrase>, ISubtitlePhraseRepository
{
    public SubtitlePhraseRepository(MovieQuotesDbContext context) : base(context)
    {
    }
}