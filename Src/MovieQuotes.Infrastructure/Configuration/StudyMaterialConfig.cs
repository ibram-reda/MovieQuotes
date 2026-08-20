namespace MovieQuotes.Infrastructure.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieQuotes.Domain.Models;

internal class StudyMaterialConfig : IEntityTypeConfiguration<StudyMaterial>
{
    public void Configure(EntityTypeBuilder<StudyMaterial> builder)
    {
        builder.HasKey(a => a.Id);
        builder.HasOne(a => a.Phrase);
        builder.Property(a => a.Content)
            .HasMaxLength(100);

        builder.Property(a => a.Definition)
            .HasMaxLength(500);

        builder.Property(a => a.Pronunciation)
            .HasMaxLength(70);

        builder.Property(a => a.Level)
            .HasMaxLength(2);

        builder.Property(a => a.Synonyms)
            .HasMaxLength(300);
        
        builder.Property(a => a.Examples)
            .HasMaxLength(500);

        builder.Property(a => a.PartOfSpeech)
            .HasMaxLength(50);

        builder.Property(a => a.ContentArabicTranslation)
            .HasMaxLength(100);

        builder.Property(a => a.ArPhraseTranslation)
            .HasMaxLength(300);

        builder.Property(a => a.CreatedDate).ValueGeneratedOnAdd();

        builder.Property(a=>a.Notes)
            .HasMaxLength(500);

        builder.Property(a=>a.Tags)
            .HasMaxLength(120);

        builder.Property(a => a.Origin)
            .HasMaxLength(100);

    }
}
