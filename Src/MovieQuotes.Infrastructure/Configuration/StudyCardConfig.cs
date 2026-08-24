namespace MovieQuotes.Infrastructure.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieQuotes.Domain.Models;

internal class StudyCardConfig : IEntityTypeConfiguration<StudyCard>
{
    public void Configure(EntityTypeBuilder<StudyCard> builder)
    {
        builder.HasKey(a => a.Id);
        builder.HasOne(a => a.StudyMaterial)
            .WithMany(m=>m.StudyCards)
            .HasForeignKey(a => a.StudyMaterialId)
            .OnDelete(DeleteBehavior.Cascade);


        builder.Property(a => a.IsActive).IsRequired();

        builder.Property(a => a.CreatedAt).ValueGeneratedOnAdd();

        builder.Property(a => a.ModifiedAt).ValueGeneratedOnAddOrUpdate();

    }
}
