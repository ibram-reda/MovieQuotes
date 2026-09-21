using System.Text;
using MovieQuotes.Subtitles.Models;
using MovieQuotes.Subtitles.Srt;

namespace MovieQuotes.Subtitles.Tests.Srt;

public sealed class SrtSubtitleParserTests
{
    private readonly SrtSubtitleParser _parser = new();

    [Fact]
    public async Task Parse_ValidSubtitle_ReturnsSubtitle()
    {
        const string content =
            """
            1
            00:00:01,000 --> 00:00:03,500
            Hello, how are you?
            """;

        var result = await Parse(content);

        var subtitle = Assert.Single(result);

        Assert.Equal(1, subtitle.Index);
        Assert.Equal(TimeSpan.FromSeconds(1), subtitle.Start);
        Assert.Equal(TimeSpan.FromMilliseconds(3500), subtitle.End);
        Assert.Equal("Hello, how are you?", subtitle.Text);
    }

    [Fact]
    public async Task Parse_MultipleSubtitles_ReturnsAllSubtitles()
    {
        const string content =
            """
            1
            00:00:01,000 --> 00:00:03,000
            Hello.

            2
            00:00:04,000 --> 00:00:06,500
            How are you?
            """;

        var result = await Parse(content);

        Assert.Equal(2, result.Count);

        Assert.Equal(1, result[0].Index);
        Assert.Equal("Hello.", result[0].Text);

        Assert.Equal(2, result[1].Index);
        Assert.Equal("How are you?", result[1].Text);
    }

    [Fact]
    public async Task Parse_MultilineSubtitle_PreservesLines()
    {
        const string content =
            """
            1
            00:00:01,000 --> 00:00:05,000
            Hello.
            How are you?
            """;

        var result = await Parse(content);

        var subtitle = Assert.Single(result);

        Assert.Equal(
            "Hello." + Environment.NewLine + "How are you?",
            subtitle.Text);
    }

    [Fact]
    public async Task Parse_MixedEmptyandNonEmptyEnteries_parsesCorrectly()
    {
        const string content =
            """
            43
            00:04:27,642 --> 00:04:34,288
            <i>♪ The circle of life ♪</i>

            44
            00:04:34,289 --> 00:04:39,511


            45
            00:04:55,920 --> 00:04:59,260
            Life's not fair, is it?
            """;

        var result = await Parse(content);

        Assert.Equal(3,result.Count);
        Assert.Equal("<i>♪ The circle of life ♪</i>",result[0].Text);
        Assert.Empty(result[1].Text);
        Assert.Equal("Life's not fair, is it?",result[2].Text);
    }

    [Fact]
    public async Task Parse_CrlfLineEndings_ParsesCorrectly()
    {
        const string content =
            "1\r\n" +
            "00:00:01,000 --> 00:00:03,000\r\n" +
            "Hello.\r\n" +
            "\r\n" +
            "2\r\n" +
            "00:00:04,000 --> 00:00:06,000\r\n" +
            "World.\r\n";

        var result = await Parse(content);

        Assert.Equal(2, result.Count);

        Assert.Equal("Hello.", result[0].Text);
        Assert.Equal("World.", result[1].Text);
    }

    [Fact]
    public async Task Parse_LfLineEndings_ParsesCorrectly()
    {
        const string content =
            "1\n" +
            "00:00:01,000 --> 00:00:03,000\n" +
            "Hello.\n\n" +
            "2\n" +
            "00:00:04,000 --> 00:00:06,000\n" +
            "World.\n";

        var result = await Parse(content);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task Parse_EmptyContent_ReturnsEmptyList()
    {
        var result = await Parse(string.Empty);

        Assert.Empty(result);
    }

    [Fact]
    public async Task Parse_WhitespaceContent_ReturnsEmptyList()
    {
        var result = await Parse("   \r\n\r\n   ");

        Assert.Empty(result);
    }

    [Fact]
    public async Task Parse_ExtraBlankLinesBetweenSubtitles_ParsesCorrectly()
    {
        const string content =
            """
            
            
            1
            00:00:01,000 --> 00:00:03,000
            Hello.


            
            
            2
            00:00:04,000 --> 00:00:06,000
            World.
            
            
            """;

        var result = await Parse(content);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task Parse_DotMilliseconds_ParsesCorrectly()
    {
        const string content =
            """
            1
            00:00:01.500 --> 00:00:03.750
            Hello.
            """;

        var result = await Parse(content);

        var subtitle = Assert.Single(result);

        Assert.Equal(
            TimeSpan.FromMilliseconds(1500),
            subtitle.Start);

        Assert.Equal(
            TimeSpan.FromMilliseconds(3750),
            subtitle.End);
    }

    [Fact]
    public async Task Parse_HourLessThanTen_ParsesCorrectly()
    {
        const string content =
            """
            1
            01:02:03,456 --> 01:02:05,789
            Hello.
            """;

        var result = await Parse(content);

        var subtitle = Assert.Single(result);

        Assert.Equal(
            new TimeSpan(0, 1, 2, 3, 456),
            subtitle.Start);

        Assert.Equal(
            new TimeSpan(0, 1, 2, 5, 789),
            subtitle.End);
    }

    [Fact]
    public async Task Parse_MissingTimestamp_ThrowsFormatException()
    {
        const string content =
            """
            1
            """;

        var exception = await Assert.ThrowsAsync<FormatException>(
            async () => await Parse(content));

        Assert.Contains(
            "missing its timestamp",
            exception.Message);
    }

    [Fact]
    public async Task Parse_InvalidIndex_ThrowsFormatException()
    {
        const string content =
            """
            abc
            00:00:01,000 --> 00:00:03,000
            Hello.
            """;

        var exception = await Assert.ThrowsAsync<FormatException>(
            async () => await Parse(content));

        Assert.Contains(
            "Invalid subtitle index",
            exception.Message);
    }

    [Fact]
    public async Task Parse_InvalidTimestamp_ThrowsFormatException()
    {
        const string content =
            """
            1
            invalid --> 00:00:03,000
            Hello.
            """;

        var exception = await Assert.ThrowsAsync<FormatException>(
            async () => await Parse(content));

        Assert.Contains(
            "invalid timestamp",
            exception.Message,
            StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Parse_MissingTimestampSeparator_ThrowsFormatException()
    {
        const string content =
            """
            1
            00:00:01,000 00:00:03,000
            Hello.
            """;

        var exception = await Assert.ThrowsAsync<FormatException>(
            async () => await Parse(content));

        Assert.Contains(
            "invalid timestamp line",
            exception.Message,
            StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Parse_EndBeforeStart_ThrowsFormatException()
    {
        const string content =
            """
            1
            00:00:05,000 --> 00:00:03,000
            Hello.
            """;

        var exception =await Assert.ThrowsAsync<FormatException>(
            async () => await Parse(content));

        Assert.Contains(
            "end time before its start time",
            exception.Message);
    }

    [Fact]
    public async Task Parse_PreservesSubtitleText()
    {
        const string content =
            """
            1
            00:00:01,000 --> 00:00:03,000
            Hello,   how are you?
            """;

        var result = await Parse(content);

        var subtitle = Assert.Single(result);

        Assert.Equal(
            "Hello,   how are you?",
            subtitle.Text);
    }

    [Fact]
    public async Task Parse_SubtitleContainingArrow_PreservesText()
    {
        const string content =
            """
            1
            00:00:01,000 --> 00:00:04,000
            I said --> something.
            """;

        var result = await Parse(content);

        var subtitle = Assert.Single(result);

        Assert.Equal(
            "I said --> something.",
            subtitle.Text);
    }

    [Fact]
    public async Task Parse_Utf8Bom_ParsesCorrectly()
    {
        const string content =
            """
            1
            00:00:01,000 --> 00:00:03,000
            Hello.
            """;

        var bytes = Encoding.UTF8.GetPreamble()
            .Concat(Encoding.UTF8.GetBytes(content))
            .ToArray();

        using var stream = new MemoryStream(bytes);

        var result = await _parser.Parse(stream);

        var subtitle = Assert.Single(result);

        Assert.Equal(1, subtitle.Index);
        Assert.Equal("Hello.", subtitle.Text);
    }

    [Fact]
    public async Task Parse_NullStream_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(
            async () => await _parser.Parse(null!));
    }

    private async Task<IReadOnlyList<SubtitleEntry>> Parse(string content)
    {
        using var stream = new MemoryStream(
            Encoding.UTF8.GetBytes(content));

        return await _parser.Parse(stream);
    }
}