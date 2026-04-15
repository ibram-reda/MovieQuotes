namespace MovieQuotes.Infrastructure.Repositories;


using Microsoft.EntityFrameworkCore;
using MovieQuotes.Domain.Interfaces;
using MovieQuotes.Domain.Models;


public class MovieRepository : GenericRepository<Movie>, IMovieRepository
{
    public MovieRepository(MovieQuotesDbContext context) : base(context)
    {
    }
}