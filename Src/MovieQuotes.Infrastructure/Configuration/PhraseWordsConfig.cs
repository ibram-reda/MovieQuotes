namespace MovieQuotes.Infrastructure.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieQuotes.Domain.Models;
using System;


internal class PhraseWordsConfig : IEntityTypeConfiguration<PhraseWords>
{
    public void Configure(EntityTypeBuilder<PhraseWords> builder)
    {        
        builder.HasKey(a => new {a.PhraseId,a.WordId,a.Index});
        builder.HasIndex(a => new {a.PhraseId,a.WordId,a.Index});
    }
}
