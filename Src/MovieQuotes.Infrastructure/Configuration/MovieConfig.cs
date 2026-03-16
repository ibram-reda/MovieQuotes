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
        builder.HasAlternateKey(a => a.FolderName);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.FolderName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.NameId)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(a => a.IMDBId).HasMaxLength(12);

        builder.Property(x => x.Description).HasMaxLength(700);
 
        builder.Property(a => a.BaseFolderDir).HasMaxLength(400);
        builder.Property(a => a.VideoFilePath).HasMaxLength(200);
        builder.Property(a => a.CoverFilePath).HasMaxLength(200); 
        builder.Property(a => a.AddedDate).ValueGeneratedOnAdd();
        builder.Property(a => a.Year).HasDefaultValue(0);
    }
}
