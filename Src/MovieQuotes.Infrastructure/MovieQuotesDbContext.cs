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
    public DbSet<Genre> Genres => Set<Genre>();

    public DbSet<SubtitlePhrase> SubtitlePhrases => Set<SubtitlePhrase>();

    public DbSet<PhraseWords> PhraseWords => Set<PhraseWords>();
    public DbSet<Word> Word => Set<Word>();

    public DbSet<StudyPhrase> StudyPhrases => Set<StudyPhrase>();
    public DbSet<StudyCard> StudyCards => Set<StudyCard>();
    public DbSet<StudyMaterial> StudyMaterials => Set<StudyMaterial>();
    public DbSet<CardProgress> CardProgresses => Set<CardProgress>();

    public DbSet<StudyPhraseProgress> StudyPhraseProgress  => Set<StudyPhraseProgress>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new MovieConfig());
        modelBuilder.ApplyConfiguration(new SubtitleConfig());
        modelBuilder.ApplyConfiguration(new PhraseWordsConfig());
        modelBuilder.ApplyConfiguration(new WordConfig());
        modelBuilder.ApplyConfiguration(new StudyPhraseConfig());
        modelBuilder.ApplyConfiguration(new StudyPhraseProgressConfig());  
        modelBuilder.ApplyConfiguration(new GenreConfig());   
        modelBuilder.ApplyConfiguration(new StudyMaterialConfig());  
        modelBuilder.ApplyConfiguration(new StudyCardConfig());  
        modelBuilder.ApplyConfiguration(new CardProgressConfig());  

    }
}
