namespace MovieQuotes.Application.Operations.CommandHandlers;

using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Models;
using MovieQuotes.Application.Operations.Commands;
using MovieQuotes.Domain.Models;
using MovieQuotes.Infrastructure;
using System.Text.RegularExpressions;

internal class CleanDatabaseCommandHandler : IRequestHandler<CleanDatabaseCommand, OperationResult<string>>
{
    private readonly MovieQuotesDbContext dbContext;

    public CleanDatabaseCommandHandler(MovieQuotesDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public async Task<OperationResult<string>> Handle(CleanDatabaseCommand request, CancellationToken cancellationToken)
    {
         var result = new OperationResult<string>();
        var movies = await this.dbContext.Movies.ToListAsync();
        foreach (var movie in movies) 
        {
            if (movie.Year != 0) continue;

            movie.Year = GetYearFromTitle(movie.Title);
        }
        var effected = await this.dbContext.SaveChangesAsync();
        result.Payload = $"effected rows count : {effected} row ";
        return result;
    }

    private static int GetYearFromTitle(string title)
    {
        Regex regex = new Regex(@"\(([0-9]{4})\)$");
        var x = regex.Match(title).Groups[1].Value;
        return int.Parse(x ?? "0");
    }
}
