namespace MovieQuotes.Infrastructure;

using Microsoft.EntityFrameworkCore;
using MovieQuotes.Domain.Models;
using MovieQuotes.Infrastructure.Configuration;

public class MovieQuotesDbContext : DbContext
{
    public MovieQuotesDbContext()
    {        
    }

    public MovieQuotesDbContext(DbContextOptions options) : base(options)
    {
    }


    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<SubtitlePhrase> SubtitlePhrases => Set<SubtitlePhrase>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new MovieConfig());
        modelBuilder.ApplyConfiguration(new SubtitleConfig());
    }
}
