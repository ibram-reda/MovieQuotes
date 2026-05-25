namespace MovieQuotes.Application.Features.Movies.Commands;

using MediatR;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.Movies.Models;

public class GenerateInfoFilesCommand : IRequest<OperationResult<bool>>
{
    /// <summary>
    /// Indicates whether to overwrite existing info files. If set to true, the command will regenerate info files for a movie,
    /// even if they already exist.
    /// </summary>
    public bool OverwriteExisting { get; set; } = false;
}