namespace MovieQuotes.Infrastructure.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieQuotes.Domain.Models;

internal class StudyMaterialPhraseConfig : IEntityTypeConfiguration<StudyMaterialPhrase>
{
    public void Configure(EntityTypeBuilder<StudyMaterialPhrase> builder)
    {
        builder.HasKey(a =>new{a.PhraseId,a.StudyMaterialId,a.Sequance});

        builder.HasOne(a => a.StudyMaterial)
            .WithMany(a=>a.Phrases)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a=>a.Phrase)
        .WithMany(a=>a.StudyMaterials)
        .OnDelete(DeleteBehavior.Restrict);

        builder.Property(a => a.ArabicTranslation)
            .HasMaxLength(300);
    }
}
