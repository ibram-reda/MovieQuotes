using Microsoft.EntityFrameworkCore;
using MovieQuotes.Domain.Models;
using MovieQuotes.Infrastructure.Configuration;

namespace MovieQuotes.Infrastructure;

public class MovieQuotesDbContext : DbContext
{
    public MovieQuotesDbContext()
    {
        
    }
    public MovieQuotesDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var cs = "Server=localhost;Database=MovieQuotesDb;Trusted_Connection=True;TrustServerCertificate=True";
        optionsBuilder.UseSqlServer(cs);
        base.OnConfiguring(optionsBuilder);
    }

    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<SubtitlePhrase> SubtitlePhrases => Set<SubtitlePhrase>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new MovieConfig());
        modelBuilder.ApplyConfiguration(new SubtitleConfig());
    }
}
