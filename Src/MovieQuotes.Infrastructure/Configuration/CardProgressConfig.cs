namespace MovieQuotes.Infrastructure.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieQuotes.Domain.Models;

internal class CardProgressConfig : IEntityTypeConfiguration<CardProgress>
{
    public void Configure(EntityTypeBuilder<CardProgress> builder)
    {
        builder.HasKey(a => a.Id);

        builder.HasAlternateKey(a=>new{a.CardId,a.ExerciseType});

        builder.HasOne(a => a.Card)
            .WithMany(a=>a.Progresses)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(a=>a.ExerciseType).IsRequired().HasDefaultValue(StudyExerciseType.Recognition);

        builder.Property(a => a.LastReviewedAt).IsRequired(false);

        builder.Property(a => a.NextReviewAt).IsRequired();

        builder.Property(a => a.Repetitions).IsRequired().HasDefaultValue(0);

        builder.Property(a => a.IntervalDays).IsRequired().HasDefaultValue(0);

        builder.Property(a => a.EaseFactor).IsRequired().HasDefaultValue(2.5);

        builder.Property(a => a.ReviewCount).IsRequired().HasDefaultValue(0);

        builder.Property(a => a.LapseCount).IsRequired().HasDefaultValue(0);

        builder.Property(a => a.CreatedDate).ValueGeneratedOnAdd();
    }
}
