namespace MovieQuotes.Application.Operations.CommandHandlers;

using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using MovieQuotes.Application.Enums;
using MovieQuotes.Application.Models;
using MovieQuotes.Application.Operations.Commands;
using MovieQuotes.Domain.Exception;
using MovieQuotes.Domain.Models;
using MovieQuotes.Infrastructure;


public class CreateMovieCommandHandler : IRequestHandler<CreateMovieCommand, OperationResult<Movie>>
{
    private readonly MovieQuotesDbContext dbContext;

    public CreateMovieCommandHandler(MovieQuotesDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public async Task<OperationResult<Movie>> Handle(CreateMovieCommand request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<Movie>();

        var movie = Movie.CreateMovie(request.Title, request.VideoLocation, request.Description,request.IMDBId,request.CoverUrl??"");

        await movie.AddSubtitleFromFileAsync(request.SubtitleLocation);

        dbContext.Movies.Add(movie);
        await dbContext.SaveChangesAsync();

        result.Payload = movie;

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

 