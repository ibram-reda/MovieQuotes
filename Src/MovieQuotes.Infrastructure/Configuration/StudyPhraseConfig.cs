namespace MovieQuotes.Infrastructure.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieQuotes.Domain.Models;

internal class StudyPhraseConfig : IEntityTypeConfiguration<StudyPhrase>
{
    public void Configure(EntityTypeBuilder<StudyPhrase> builder)
    {
        builder.HasKey(a => a.Id);
        builder.HasOne(a => a.Phrase);
        builder.Property(a => a.Content)
            .HasMaxLength(200);

        builder.Property(a => a.Translation)
            .HasMaxLength(500);

        builder.Property(a => a.Pronunciation)
            .HasMaxLength(100);

        builder.Property(a => a.Level)
            .HasMaxLength(2);

        builder.Property(a => a.Synonyms)
            .HasMaxLength(300);
        
        builder.Property(a => a.Examples)
            .HasMaxLength(500);

        builder.Property(a => a.StudyType)
            .HasMaxLength(100);

        builder.Property(a => a.ArContentTranslation)
            .HasMaxLength(120);

        builder.Property(a => a.ArPhraseTranslation)
            .HasMaxLength(500);

        builder.Property(a => a.AddedDate).ValueGeneratedOnAdd();

        builder.Property(a=>a.Notes)
            .HasMaxLength(500);

        builder.Property(a => a.Origin)
            .HasMaxLength(100);


    }
}
