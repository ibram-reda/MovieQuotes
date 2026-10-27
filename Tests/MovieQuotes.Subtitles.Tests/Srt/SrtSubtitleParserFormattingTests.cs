using MovieQuotes.Subtitles.Models;
using MovieQuotes.Subtitles.Srt;

namespace MovieQuotes.Subtitles.Tests.Srt;

public sealed class SrtSubtitleParserFormattingTests
{
    private readonly SrtSubtitleParser _parser = new();

    [Fact]
    public void Parse_EmptyInput_ReturnsEmptyDocument()
    {
        var document = _parser.Parse(string.Empty);

        Assert.Empty(document.Runs);
    }

    [Fact]
    public void Parse_TextAndHtmlEntities_ReturnsDecodedTextRun()
    {
        var document = _parser.Parse("Tom &amp; Jerry");

        Assert.Equal(
            [new SubtitleRun("Tom & Jerry")],
            document.Runs);
    }

    [Fact]
    public void Parse_NestedFormattingTags_AppliesAndRestoresStyles()
    {
        var document = _parser.Parse("<b>bold <i>bold italic</i> bold</b> plain");

        Assert.Equal(
            [
                new SubtitleRun("bold ", Bold: true),
                new SubtitleRun("bold italic", Bold: true, Italic: true),
                new SubtitleRun(" bold", Bold: true),
                new SubtitleRun(" plain")
            ],
            document.Runs);
    }

    [Fact]
    public void Parse_DeeplyNestedTags_DoesNotRecurse()
    {
        const int depth = 10000;
        var input = string.Concat(Enumerable.Repeat("<b>", depth))
                    + "text"
                    + string.Concat(Enumerable.Repeat("</b>", depth));

        var run = Assert.Single(_parser.Parse(input).Runs);

        Assert.Equal("text", run.Text);
        Assert.True(run.Bold);
    }

    [Fact]
    public void Parse_UnderlineTag_AppliesUnderlineStyle()
    {
        var document = _parser.Parse("<u>underlined</u>");

        Assert.Equal(
            [new SubtitleRun("underlined", Underline: true)],
            document.Runs);
    }

    [Fact]
    public void Parse_StrongAndEmTags_ApplyBoldAndItalicStyles()
    {
        var document = _parser.Parse("<strong>bold</strong> <em>italic</em>");

        Assert.Equal(
            [
                new SubtitleRun("bold", Bold: true),
                new SubtitleRun(" "),
                new SubtitleRun("italic", Italic: true)
            ],
            document.Runs);
    }

    [Fact]
    public void Parse_FontTag_AppliesAttributesToText()
    {
        var document = _parser.Parse(
            """<font color="#ffff80" face="Comic Sans MS" size="24">styled</font>""");

        Assert.Equal(
            [new SubtitleRun("styled", FontFamily: "Comic Sans MS", FontSize: 24, Foreground: "#ffff80")],
            document.Runs);
    }

    [Fact]
    public void Parse_LineBreaksAndBrTags_ReturnLineBreakRuns()
    {
        var document = _parser.Parse("first\r\nsecond<br/>third");

        Assert.Equal(
            [
                new SubtitleRun("first"),
                new SubtitleRun(string.Empty, IsLineBreak: true),
                new SubtitleRun("second"),
                new SubtitleRun(string.Empty, IsLineBreak: true),
                new SubtitleRun("third")
            ],
            document.Runs);
    }

    [Fact]
    public void Parse_InfoUnknownTagsAndUnclosedMarkup_PreservesVisibleText()
    {
        var document = _parser.Parse("[music] <unknown>sound</unknown> <unfinished");

        Assert.Equal(
            "[music] <unknown>sound</unknown> <unfinished",
            Assert.Single(document.Runs).Text);
    }

    [Fact]
    public void Parse_NullInput_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _parser.Parse(null!));
    }
}
