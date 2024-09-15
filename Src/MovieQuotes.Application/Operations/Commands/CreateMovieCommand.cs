namespace MovieQuotes.Application.Operations.Commands;

using MediatR;
using MovieQuotes.Application.Models;
using MovieQuotes.Domain.Models;

public class CreateMovieCommand : IRequest<OperationResult<Movie>>
{
    public CreateMovieCommand(string title, string videoLocation, string subtitlePath, string? description = null, string? iMDBId = null,string? coverUrl = null)
    {
        Title = title;
        VideoLocation = videoLocation;
        SubtitleLocation = subtitlePath;
        Description = description;
        IMDBId = iMDBId;
        CoverUrl = coverUrl;
    }
    public string Title { get; } = string.Empty;
    public string? Description { get; }
    public string? IMDBId { get; }
    public string? CoverUrl { get; }
    public string VideoLocation { get; } = string.Empty;
    public string SubtitleLocation { get; }
}
