namespace MovieQuotes.Infrastructure.Repositories;

using MovieQuotes.Domain.Interfaces;
using MovieQuotes.Domain.Models;

public class StudySessionRepository : GenericRepository<StudySession>, IStudySessionRepository
{
    public StudySessionRepository(MovieQuotesDbContext context) : base(context)
    {
    }
}