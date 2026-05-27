namespace MovieQuotes.Application.Features.Movies.CommandsHandlers;

using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using MovieQuotes.Application.Common.Enums;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.Movies.Commands;
using MovieQuotes.Application.Features.Movies.Mappings;
using MovieQuotes.Application.Features.Movies.Models;
using MovieQuotes.Application.Features.Movies.Services;
using MovieQuotes.Domain.Exception;
using MovieQuotes.Domain.Models;
using MovieQuotes.Domain.Interfaces;
using Hangfire;
using MovieQuotes.Application.Features.Movies.Jobs;

public class CreateMovieCommandHandler : IRequestHandler<CreateMovieCommand, OperationResult<MovieInfo>>
{
    private readonly IMovieQUnitOfWork movieQUnitOfWork;
    private readonly TmdbService tmdbService;
    private readonly IBackgroundJobClient backgroundJobClient;
    private readonly ILogger<CreateMovieCommandHandler> logger;

    public CreateMovieCommandHandler(IMovieQUnitOfWork movieQUnitOfWork, TmdbService tmdbService, IBackgroundJobClient backgroundJobClient,ILogger<CreateMovieCommandHandler> logger)
    {
        this.movieQUnitOfWork = movieQUnitOfWork;
        this.tmdbService = tmdbService;
        this.backgroundJobClient = backgroundJobClient;
        this.logger = logger;
    }
    public async Task<OperationResult<MovieInfo>> Handle(CreateMovieCommand request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<MovieInfo>();

        #region valdition
        if (string.IsNullOrWhiteSpace(request.IMDBId))
            result.AddError(ErrorCode.ValidationError, "IMDB ID is Required!");
        if (string.IsNullOrWhiteSpace(request.Title))
            result.AddError(ErrorCode.ValidationError, "Title is Required!");
        if (string.IsNullOrWhiteSpace(request.VideoLocation))
            result.AddError(ErrorCode.ValidationError, "VideoLocation is Required!");
        if (!File.Exists(request.VideoLocation))
            result.AddError(ErrorCode.NotFound, "VideoLocation should be a valid path on system");
        if (!string.IsNullOrWhiteSpace(request.CoverUrl))
            if (!File.Exists(request.CoverUrl))
                result.AddError(ErrorCode.ValidationError, "the providing CoverUrl is not exist on this system");
        if (result.IsError)
            return result;
        #endregion

        var vedioName = Path.GetFileName(request.VideoLocation);
        var coverName = Path.GetFileName(request.CoverUrl ?? "");

        var movie = Movie.CreateMovie(request.BaseFolder, request.FolderName, request.Title, vedioName, request.Description, request.IMDBId, coverName, request.Year);

        await this.movieQUnitOfWork.Movies.AddAsync(movie);
        await movieQUnitOfWork.SaveAsync();
        this.logger.LogInformation($"Movie '{movie.Title}' (ID: {movie.Id}) created successfully. Enqueuing metadata job...");


        // Fetch TMDb data and update the movie with the fetched data
        backgroundJobClient.Enqueue<MovieMetadataJob>(x =>  x.ProcessAsync(movie.Id));
        this.logger.LogInformation($"Metadata job enqueued for movie '{movie.Title}' (ID: {movie.Id}).");

        result.Payload = movie.ToMovieInfo();

        return result;
    }

}

public class CreateMovieCommandExceptionHandler : IRequestExceptionHandler<CreateMovieCommand, OperationResult<Movie>, Exception>
{
    private readonly ILogger<CreateMovieCommandExceptionHandler> logger;

    public CreateMovieCommandExceptionHandler(ILogger<CreateMovieCommandExceptionHandler> logger)
    {
        this.logger = logger;
    }
    public Task Handle(CreateMovieCommand request, Exception exception, RequestExceptionHandlerState<OperationResult<Movie>> state, CancellationToken cancellationToken)
    {
        logger.LogError(
            exception,
            $"--- Exception Handler: '{nameof(CreateMovieCommandExceptionHandler)}'"
            );

        var result = new OperationResult<Movie>();

        switch (exception)
        {
            case MovieNotValidException validException:
                foreach (var e in validException.ValidationErrors)
                {
                    result.AddError(ErrorCode.ValidationError, e);
                }
                break;
            default:
                var ex = exception;
                while (ex is not null)
                {
                    result.AddUnknownError(ex.Message);
                    ex = ex.InnerException;
                }
                break;
        }

        state.SetHandled(result);
        return Task.CompletedTask;
    }
}

