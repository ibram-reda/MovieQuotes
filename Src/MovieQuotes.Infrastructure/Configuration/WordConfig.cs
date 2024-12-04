namespace MovieQuotes.Infrastructure.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieQuotes.Domain.Models;


internal class WordConfig : IEntityTypeConfiguration<Word>
{
    public void Configure(EntityTypeBuilder<Word> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasAlternateKey(x => x.Text);
        builder.HasIndex(x => x.Text);

        builder.Property(a=>a.Text)
            .HasMaxLength(100);

        builder.HasMany(a => a.Phrases)
            .WithOne(pw => pw.Word);
    }
}
