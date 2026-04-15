namespace MovieQuotes.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MovieQuotes.Infrastructure;

public class MovieQoutesContextFatory : IDesignTimeDbContextFactory<MovieQuotesDbContext>
{
    public MovieQuotesDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MovieQuotesDbContext>();
        optionsBuilder.UseMySQL("Server=localhost;Database=MovieQuotesDb_Tests;uid=root;pwd=root;");

        return new MovieQuotesDbContext(optionsBuilder.Options);
    }
}