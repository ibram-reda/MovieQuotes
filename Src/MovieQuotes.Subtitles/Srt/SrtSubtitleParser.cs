using System.Globalization;
using System.Text;
using MovieQuotes.Subtitles.Abstractions;
using MovieQuotes.Subtitles.Models;

namespace MovieQuotes.Subtitles.Srt;

public sealed class SrtSubtitleParser : ISubtitleParser
{
    private const string TimestampSeparator = "-->";

    public async Task<IReadOnlyList<SubtitleEntry>> Parse(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        if (!stream.CanRead)
            throw new ArgumentException(
                "The stream must be readable.",
                nameof(stream));

        using var reader = new StreamReader(
            stream,
            Encoding.UTF8,
            detectEncodingFromByteOrderMarks: true,
            bufferSize: 4096,
            leaveOpen: true);

        var subtitles = new List<SubtitleEntry>();

        while (true)
        {
            var indexLine = await ReadNextNonEmptyLine(reader);

            if (indexLine is null)
                break;

            if (!int.TryParse(
                    indexLine,
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out var index))
            {
                throw new FormatException(
                    $"Invalid subtitle index: '{indexLine}'.");
            }

            var timestampLine = await reader.ReadLineAsync();

            if (timestampLine is null)
            {
                throw new FormatException(
                    $"Subtitle {index} is missing its timestamp.");
            }

            var (start, end) = ParseTimestampLine(timestampLine, index);

            var text = await ReadSubtitleText(reader);

            subtitles.Add(
                new SubtitleEntry(
                    index,
                    start,
                    end,
                    text));
        }

        return subtitles;
    }

    private static async Task<string?> ReadNextNonEmptyLine(StreamReader reader)
    {
        while (await reader.ReadLineAsync() is { } line)
        {
            if (!string.IsNullOrWhiteSpace(line))
                return line.Trim();
        }

        return null;
    }

    private static (TimeSpan Start, TimeSpan End) ParseTimestampLine(
        string line,
        int subtitleIndex)
    {
        var separatorIndex = line.IndexOf(
            TimestampSeparator,
            StringComparison.Ordinal);

        if (separatorIndex < 0)
        {
            throw new FormatException(
                $"Subtitle {subtitleIndex} has an invalid timestamp line: '{line}'.");
        }

        var startText = line[..separatorIndex].Trim();
        var endText = line[(separatorIndex + TimestampSeparator.Length)..].Trim();

        var start = ParseTimestamp(startText, subtitleIndex);
        var end = ParseTimestamp(endText, subtitleIndex);

        if (end < start)
        {
            throw new FormatException(
                $"Subtitle {subtitleIndex} has an end time before its start time.");
        }

        return (start, end);
    }

    private static TimeSpan ParseTimestamp(
        string value,
        int subtitleIndex)
    {
        // Standard SRT:
        // HH:mm:ss,fff
        //
        // Also accepts:
        // HH:mm:ss.fff

        value = value.Replace('.', ',');

        if (!TimeSpan.TryParseExact(
                value,
                [
                    @"hh\:mm\:ss\,fff",
                    @"h\:mm\:ss\,fff"
                ],
                CultureInfo.InvariantCulture,
                out var result))
        {
            throw new FormatException(
                $"Subtitle {subtitleIndex} has an invalid timestamp: '{value}'.");
        }

        return result;
    }

    private static async Task<string> ReadSubtitleText(StreamReader reader)
    {
        var builder = new StringBuilder();

        while (await reader.ReadLineAsync() is { } line)
        {
            // Empty line means the end of this subtitle block.
            if (string.IsNullOrWhiteSpace(line))
                break;

            if (builder.Length > 0)
                builder.AppendLine();

            builder.Append(line);
        }

        return builder.ToString();
    }
}