namespace MovieQuotes.Infrastructure.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieQuotes.Domain.Models;

internal class StudySessionCardConfig : IEntityTypeConfiguration<StudySessionCard>
{
    public void Configure(EntityTypeBuilder<StudySessionCard> builder)
    {
        builder.HasKey(card => card.Id);
        builder.Property(card => card.StudyCardId).IsRequired();
        builder.Property(card => card.Order).IsRequired();
        builder.Property(card => card.IsCompleted).IsRequired();
        builder.Property(card => card.CompletedAt).IsRequired(false);
        builder.HasIndex(card => new { card.StudySessionId, card.Order }).IsUnique();
    }
}