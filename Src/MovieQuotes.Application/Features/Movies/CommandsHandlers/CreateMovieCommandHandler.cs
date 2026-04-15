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

public class CreateMovieCommandHandler : IRequestHandler<CreateMovieCommand, OperationResult<MovieInfo>>
{
    private readonly IMovieQUnitOfWork movieQUnitOfWork;

    public CreateMovieCommandHandler(IMovieQUnitOfWork movieQUnitOfWork)
    {
        this.movieQUnitOfWork = movieQUnitOfWork;
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

        var movie = Movie.CreateMovie(request.BaseFolder, request.FolderName, request.Title, request.VideoLocation, request.Description, request.IMDBId, request.CoverUrl ?? "", request.Year);

        await this.movieQUnitOfWork.Movies.AddAsync(movie);
        await movieQUnitOfWork.SaveAsync();

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

