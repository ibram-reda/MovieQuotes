namespace MovieQuotes.Infrastructure.Repositories;


using Microsoft.EntityFrameworkCore;
using MovieQuotes.Domain.Interfaces;
using MovieQuotes.Domain.Models;

public class GenreRepository : GenericRepository<Genre>, IGenreRepository
{
    public GenreRepository(MovieQuotesDbContext context) : base(context)
    {
    }
}