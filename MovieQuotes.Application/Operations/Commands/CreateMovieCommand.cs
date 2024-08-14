namespace MovieQuotes.Application.Operations.Commands;

using MediatR;
using MovieQuotes.Application.Models;
using MovieQuotes.Domain.Models;
using System.Diagnostics;

public class CreateMovieCommand : IRequest<OperationResult<Movie>>
{
    public CreateMovieCommand(string title, string videoLocation, string subtitlePath, string? description=null)
    {
        Title = title;
        VideoLocation = videoLocation;
        SubtitleLocation = subtitlePath;
        Description = description;
    }
    public string Title { get; } = string.Empty;
    public string? Description { get; }
    public string VideoLocation { get; } = string.Empty;
    public string SubtitleLocation { get; }
}
