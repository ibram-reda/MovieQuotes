using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieQuotes.Domain.Models;

namespace MovieQuotes.Infrastructure.Configuration;

public class SubtitleConfig : IEntityTypeConfiguration<SubtitlePhrase>
{
    public void Configure(EntityTypeBuilder<SubtitlePhrase> builder)
    {
        builder.HasKey(a => new {a.MovieId,a.Sequence});
                



        builder.HasIndex(a => a.Text);

        builder.Property(a=>a.Text)
            .IsRequired()
            .HasMaxLength(700);

        builder.Property(a=>a.VideoClipPath)
            .HasMaxLength(700);


    }
}
