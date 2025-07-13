namespace MovieQuotes.Application.Tests.Common;

using MovieQuotes.Application.Common.Services;

public class SrtReaderTests
{
    [Fact]
    public void Parse_ValidSrtContent_ReturnsCorrectSubtitlePhrases()
    {
        string srtContent = """
            1
            00:00:01,000 --> 00:00:05,000
            Hello, world!

            2
            00:00:06,000 --> 00:00:10,000
            This is a test subtitle.
            """;
        var result = SrtReader.GetPhrases(srtContent);
        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].Sequence);
        Assert.Equal(TimeSpan.FromSeconds(1), result[0].StartTime);
        Assert.Equal(TimeSpan.FromSeconds(5), result[0].EndTime);
        Assert.Equal("Hello, world!", result[0].Text);
        Assert.Equal(2, result[1].Sequence);
        Assert.Equal(TimeSpan.FromSeconds(6), result[1].StartTime);
        Assert.Equal(TimeSpan.FromSeconds(10), result[1].EndTime);
        Assert.Equal("This is a test subtitle.", result[1].Text);
    }

    [Fact]
    public void Parse_SrtWithMultipleLines_ReturnsCorrectSubtitlePhrases()
    {
        string srtContent = """
            1
            00:00:01,000 --> 00:00:05,000
            Hello, world!
            This is a test.

            2
            00:00:06,000 --> 00:00:10,000
            Another subtitle line.
            """;
        var result = SrtReader.GetPhrases(srtContent);
        Assert.Equal(2, result.Count);
        Assert.Equal("Hello, world!\r\nThis is a test.", result[0].Text);
        Assert.Equal("Another subtitle line.", result[1].Text);
    }

    [Fact]
    public void Parse_ReSequenceDuplicates_ReturnsCorrectlyReSequencedPhrases()
    {
        string srtContent = """
            1
            00:00:01,000 --> 00:00:05,000
            First subtitle.

            2
            00:00:06,000 --> 00:00:10,000
            Second subtitle.

            1
            00:00:11,000 --> 00:00:15,000
            Duplicate first subtitle.
            """;
        var result = SrtReader.GetPhrases(srtContent);
        Assert.Equal(3, result.Count);
        Assert.Equal(1, result[0].Sequence);
        Assert.Equal(2, result[1].Sequence);
        Assert.Equal(3, result[2].Sequence);
        Assert.Equal("Duplicate first subtitle.", result[2].Text);
    }

    [Fact]
    public void Parse_InvalidSrtContent_ThrowsException()
    {
        string srtContent = """
            Invalid content
            """;
        Assert.Throws<Exception>(() => SrtReader.GetPhrases(srtContent));
    }

    [Fact]
    public void Parse_EmptySrtContent_ThrowsException()
    {
        string srtContent = "";
        Assert.Throws<Exception>(() => SrtReader.GetPhrases(srtContent));
    }

    [Fact]
    public void Parse_SrtWithMarkupTags_GetItCorrect()
    {
        string srtContent = """
            1
            00:00:01,000 --> 00:00:05,000
            <i>Hello</i>,
            <b>world</b>!

            2
            00:00:06,000 --> 00:00:10,000
            This is a <u>test</u> subtitle.
            """;
        var result = SrtReader.GetPhrases(srtContent);
        Assert.Equal("<i>Hello</i>,\r\n<b>world</b>!", result[0].Text);
        Assert.Equal("This is a <u>test</u> subtitle.", result[1].Text);
    }
}
