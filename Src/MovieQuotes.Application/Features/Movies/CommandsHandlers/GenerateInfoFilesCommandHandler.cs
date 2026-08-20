namespace MovieQuotes.Application.Features.Movies.CommandsHandlers;

using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using MovieQuotes.Application.Common.Enums;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.Movies.Commands;
using MovieQuotes.Application.Features.Movies.Mappings;
using MovieQuotes.Application.Features.Movies.Models;
using MovieQuotes.Domain.Exception;
using MovieQuotes.Domain.Models;
using MovieQuotes.Domain.Interfaces;
using MovieQuotes.Application.Features.Movies.Services;

public class GenerateInfoFilesCommandHandler : IRequestHandler<GenerateInfoFilesCommand, OperationResult<bool>>
{
    private readonly IMovieQUnitOfWork movieQUnitOfWork;
    private readonly TmdbService tmdbService;

    public GenerateInfoFilesCommandHandler(IMovieQUnitOfWork movieQUnitOfWork, TmdbService tmdbService)
    {
        this.movieQUnitOfWork = movieQUnitOfWork;
        this.tmdbService = tmdbService;
    } 
    public async Task<OperationResult<bool>> Handle(GenerateInfoFilesCommand request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<bool>();

        var movies = await this.movieQUnitOfWork.Movies.GetAllAsync();

        foreach (var movie in movies)
        {
            try
            { 
                var stopWatch = System.Diagnostics.Stopwatch.StartNew();
                Console.Write($"{movie.Id}. {movie.Title} ...");
                await movie.GenerateInfoFileAsync(request.OverwriteExisting);                   

                stopWatch.Stop();
                Console.WriteLine($"Finished processing, Time taken: {stopWatch.ElapsedMilliseconds} ms");
            }
            catch (Exception ex)
            {
                // Log the error and continue with the next movie
                Console.WriteLine($"Error processing movie '{movie.Title}': {ex.Message}");
            }
            
        }

        result.Payload = true;

        return result;
    }

}

 