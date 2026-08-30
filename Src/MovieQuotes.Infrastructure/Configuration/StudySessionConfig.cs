namespace MovieQuotes.Infrastructure.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieQuotes.Domain.Models;

internal class StudySessionConfig : IEntityTypeConfiguration<StudySession>
{
    public void Configure(EntityTypeBuilder<StudySession> builder)
    {
        builder.HasKey(session => session.Id);
        builder.Property(session => session.ExerciseType).IsRequired();
        builder.Property(session => session.StartedAt).IsRequired();
        builder.Property(session => session.CompletedAt).IsRequired(false);
        builder.Property(session => session.CurrentCardPosition).IsRequired();

        builder.HasMany(session => session.Cards)
            .WithOne()
            .HasForeignKey(card => card.StudySessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(session => session.CompletedAt);
    }
}