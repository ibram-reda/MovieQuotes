namespace MovieQuotes.Application.Operations.CommandHandlers;

using MediatR;
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

        try
        {
            var movie = Movie.CreateMovie(request.Title, request.VideoLocation, request.Description);

            
            await movie.AddSubtitleFromFileAsync(request.SubtitleLocation);

            dbContext.Movies.Add(movie);
            await dbContext.SaveChangesAsync();

            result.Payload = movie;
        }
        catch (MovieNotValidException ex)
        {
            foreach (var e in ex.ValidationErrors)
            {
                result.AddError(ErrorCode.ValidationError, e);
            }
        }
        catch (Exception ex)
        {
            result.AddUnknownError(ex.Message);
        }
        return result;
    }
}
