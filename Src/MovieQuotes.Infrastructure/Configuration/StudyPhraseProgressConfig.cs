namespace MovieQuotes.Infrastructure.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieQuotes.Domain.Models;


public class StudyPhraseProgressConfig : IEntityTypeConfiguration<StudyPhraseProgress>
{
    public void Configure(EntityTypeBuilder<StudyPhraseProgress> builder)
    {
         builder.HasOne(p => p.StudyPhrase)
            .WithOne(s => s.Progress)
            .HasForeignKey<StudyPhraseProgress>(p => p.StudyPhraseId);

        builder.Property(p => p.EaseFactor)
            .HasDefaultValue(2.5);

        builder.HasKey(p => p.Id);
    }
}
