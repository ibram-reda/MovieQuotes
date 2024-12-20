namespace MovieQuotes.Application.Features.Movies.Commands;

using MediatR;
using MovieQuotes.Application.Models;
using MovieQuotes.Domain.Models;

public class CreateMovieCommand : IRequest<OperationResult<Movie>>
{
    public CreateMovieCommand(string title, int year, string videoLocation, string? description = null, string? iMDBId = null, string? coverUrl = null)
    {
        Title = title;
        VideoLocation = videoLocation; 
        Description = description;
        IMDBId = iMDBId;
        CoverUrl = coverUrl;
        Year = year;
    }
    public string Title { get; } = string.Empty;
    public string? Description { get; }
    public string? IMDBId { get; }
    public string? CoverUrl { get; }
    public string VideoLocation { get; } = string.Empty; 
    public int Year { get; }
}
