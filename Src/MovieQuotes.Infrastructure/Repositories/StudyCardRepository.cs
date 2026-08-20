namespace MovieQuotes.Infrastructure.Repositories;


using Microsoft.EntityFrameworkCore;
using MovieQuotes.Domain.Interfaces;
using MovieQuotes.Domain.Models;


public class StudyCardRepository : GenericRepository<StudyCard>, IStudyCardRepository
{
    public StudyCardRepository(MovieQuotesDbContext context) : base(context)
    {
    }

     
}