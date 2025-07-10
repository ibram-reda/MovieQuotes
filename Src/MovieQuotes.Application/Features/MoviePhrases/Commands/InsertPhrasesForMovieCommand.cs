namespace MovieQuotes.Application.Features.MoviePhrases.Commands;

using MediatR;
using MovieQuotes.Application.Common.Models;

 
/// <summary>
/// Represents a command to insert phrases for a specific movie, optionally using a subtitle file.
/// </summary>
/// <remarks>This command is used to process and insert English phrases associated with a movie into the database. If <see
/// cref="SubtitleLocation"/> is null or empty, the command will attempt to locate subtitle files in the movie's
/// folder.</remarks>
public class InsertPhrasesForMovieCommand : IRequest<OperationResult<bool>>
{
    public long MovieId { get; set; }

    /// <summary>
    /// Gets or sets the file path of the subtitle if it's null or empty, it will search for subtitles in the movie folder.
    /// </summary>
    public string? SubtitleLocation { get; set; } = "";
     
}
