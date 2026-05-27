namespace MovieQuotes.Application.Features.Movies.Jobs;

using Microsoft.Extensions.Logging;
using MovieQuotes.Application.Features.Movies.Services;
using MovieQuotes.Domain.Interfaces;

public class MovieMetadataJob
{
    private readonly IMovieQUnitOfWork unitOfWork;
    private readonly TmdbService tmdbService;
    private readonly ILogger<MovieMetadataJob> logger;

    public MovieMetadataJob(
        IMovieQUnitOfWork unitOfWork,
        TmdbService tmdbService,
        ILogger<MovieMetadataJob> logger)
    {
        this.unitOfWork = unitOfWork;
        this.tmdbService = tmdbService;
        this.logger = logger;
    }

    public async Task ProcessAsync(int movieId)
    {
        var movie = await unitOfWork.Movies.GetByIdAsync(movieId);

        if (movie == null)
        {
            logger.LogError($"Movie with ID {movieId} not found.");
            return;
        }

        // Fetch TMDb data
        var tmdbData = await tmdbService.FetchTmdbDataAsync(movie);

        movie.UpdateTMDbData(tmdbData);

        // Genres
        var genres = await tmdbService.GetMovieGenresAsync(movie);

        foreach (var genreId in genres)
        {
            var genre = await unitOfWork.Genres.GetByIdAsync(genreId);

            if (genre != null)
                movie.AddGenre(genre);
        }

        await unitOfWork.Movies.UpdateAsync(movie);
        await unitOfWork.SaveAsync();
        logger.LogInformation($"Finished processing metadata for movie '{movie.Title}' (ID: {movie.Id}).");
        logger.LogInformation("Starting backdrop and poster download...");
        // Downloads
        await movie.DownloadBackDropAsync();
        await movie.DownloadPosterAsync();
        logger.LogInformation($"Finished downloading backdrop and poster for movie '{movie.Title}' (ID: {movie.Id}).");

        
    }
}