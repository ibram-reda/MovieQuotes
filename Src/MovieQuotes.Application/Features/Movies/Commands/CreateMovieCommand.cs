namespace MovieQuotes.Application.Features.Movies.Commands;

using MediatR;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Domain.Models;

public class CreateMovieCommand : IRequest<OperationResult<Movie>>
{
    public CreateMovieCommand(string baseFolder,string title, int year, string videoLocation, string? description = null, string? iMDBId = null, string? coverUrl = null)
    {
        BaseFolder = baseFolder;
        Title = title;
        VideoLocation = videoLocation; 
        Description = description;
        IMDBId = iMDBId;
        CoverUrl = coverUrl;
        Year = year;
    }

    public string BaseFolder { get; set; }
    public string Title { get; } = string.Empty;
    public string? Description { get; }
    public string? IMDBId { get; }
    public string? CoverUrl { get; }
    public string VideoLocation { get; } = string.Empty; 
    public int Year { get; }
}
