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

    public DbSet<PhraseWords> PhraseWords => Set<PhraseWords>();
    public DbSet<Word> Word => Set<Word>();

    public DbSet<StudyPhrase> StudyPhrases => Set<StudyPhrase>();

    public DbSet<StudyPhraseProgress> StudyPhraseProgress  => Set<StudyPhraseProgress>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new MovieConfig());
        modelBuilder.ApplyConfiguration(new SubtitleConfig());
        modelBuilder.ApplyConfiguration(new PhraseWordsConfig());
        modelBuilder.ApplyConfiguration(new WordConfig());
        modelBuilder.ApplyConfiguration(new StudyPhraseConfig());

        modelBuilder.Entity<StudyPhraseProgress>()
            .HasOne(p => p.StudyPhrase)
            .WithOne(s => s.Progress)
            .HasForeignKey<StudyPhraseProgress>(p => p.StudyPhraseId);

        modelBuilder.Entity<StudyPhraseProgress>()
            .Property(p => p.EaseFactor)
            .HasDefaultValue(2.5);

        modelBuilder.Entity<StudyPhraseProgress>().HasKey(p => p.Id);

    }
}
