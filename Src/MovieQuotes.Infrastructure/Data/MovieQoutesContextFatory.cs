namespace MovieQuotes.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MovieQuotes.Infrastructure;

public class MovieQoutesContextFatory : IDesignTimeDbContextFactory<MovieQuotesDbContext>
{
    public MovieQuotesDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MovieQuotesDbContext>();
        var cs = args.Length > 0 ? args[0] : "Data Source=MovieQuotes.Dev.db";
        optionsBuilder.UseSqlite(cs);
        return new MovieQuotesDbContext(optionsBuilder.Options);
    }
}