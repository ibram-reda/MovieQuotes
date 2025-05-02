namespace MovieQuotes.Application.Features.SubtitleFiles.Commands;

using MediatR;
using MovieQuotes.Application.Common.Models;
using System.Text;

public class SubtitleResyncCommand : IRequest<OperationResult<Unit>>
{
    /// <summary>
    /// File Path of subtitle.
    /// </summary>
    public string SubtitleFilePath { get; set; } = string.Empty;

    /// <summary>
    /// Name of new Subtitle File.<br/>
    /// <remark>
    /// it will be output in the same directory as the subtitleFilePath.
    /// </remark>
    /// </summary>
    public string OutPutFileName { get; set; } = string.Empty;

    /// <summary>
    /// how many time shift in ms could be +ve or -ve
    /// </summary>
    public int TimeShift { get; set; } = 0;

    public Encoding InputFileEncoding { get; set; } = Encoding.UTF8;

    public Encoding OutputFileEncoding { get; set; } = Encoding.UTF8;
}
