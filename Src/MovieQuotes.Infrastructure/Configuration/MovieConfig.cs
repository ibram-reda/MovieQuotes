namespace MovieQuotes.Infrastructure.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieQuotes.Domain.Models;


public class MovieConfig : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasAlternateKey(a => a.Title);
        builder.HasAlternateKey(a => a.NameId);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(x => x.NameId)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(a => a.IMDBId).HasMaxLength(12);

        builder.Property(x => x.Description).HasMaxLength(700);

        builder.Property(a => a.LocalPath).HasMaxLength(700);
        builder.Property(a => a.BaseFolderDir).HasMaxLength(700);
        builder.Property(a => a.CoverUrl).HasMaxLength(700);
        builder.Property(a => a.AddedDate).ValueGeneratedOnAdd();
        builder.Property(a => a.Year).HasDefaultValue(0);
    }
}
