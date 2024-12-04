namespace MovieQuotes.Application.Operations.CommandHandlers;

using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MovieQuotes.Application.Enums;
using MovieQuotes.Application.Models;
using MovieQuotes.Application.Operations.Commands;
using MovieQuotes.Domain.Exception;
using MovieQuotes.Domain.Models;
using MovieQuotes.Infrastructure;
using System.Text.RegularExpressions;

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

        var movie = Movie.CreateMovie(request.Title, request.VideoLocation, request.Description, request.IMDBId, request.CoverUrl ?? "",request.Year);

        await movie.AddSubtitleFromFileAsync(request.SubtitleLocation);

        dbContext.Movies.Add(movie);
        await dbContext.SaveChangesAsync();
         
        await AddWordsAsync(movie.Subtitles);

        result.Payload = movie;

        return result;
    }

    private async Task AddWordsAsync(List<SubtitlePhrase> phrases)
    {
        foreach (var phrase in phrases)
        {
            if (phrase.PhraseWords.Any()) continue;
            var words = phrase.Text.Split(' ');
            int i = 0;
            foreach (var word in words)
            {
                var normalizedWord = Normalize(word);
                var w = await this.dbContext.Word.FirstOrDefaultAsync(a => a.Text == normalizedWord);
                if (w is null)
                {
                    w = Word.CreateWord(normalizedWord);
                    this.dbContext.Word.Add(w);
                    await this.dbContext.SaveChangesAsync();
                }
                var pw = PhraseWords.Create(phrase, w, i++);
                phrase.PhraseWords.Add(pw);
            }
            await this.dbContext.SaveChangesAsync();
        }
    }

    Regex rgx = new Regex("^[^a-zA-Z0-9]+|[^a-zA-Z0-9]+$");
    private string Normalize(string word)
    {
     return rgx.Replace(word, ""); 
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

 