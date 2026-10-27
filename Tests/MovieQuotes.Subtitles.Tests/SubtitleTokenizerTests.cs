using MovieQuotes.Subtitles.Models;

namespace MovieQuotes.Subtitles.Tests;

public sealed class SubtitleTokenizerTests
{
    [Fact]
    public void Tokenize_EmptyInput_ReturnsNoTokens()
    {
        Assert.Empty(SubtitleTokenizer.Tokenize(string.Empty));
    }

    [Fact]
    public void Tokenize_PlainText_ReturnsTextToken()
    {
        var tokens = SubtitleTokenizer.Tokenize("Hello, world!");


        Assert.Collection(
            tokens,
            token => Assert.Equal(new SubtitleToken(SubtitleTokenType.Text, "Hello, world!"), token));
    }

    [Fact]
    public void Tokenize_TextWithTagsAndInfo_ReturnsTokensInOrder()
    {
        var tokens = SubtitleTokenizer.Tokenize("Hello <i>world</i> [music]");

        Assert.Collection(
            tokens,
            token => Assert.Equal(new SubtitleToken(SubtitleTokenType.Text, "Hello "), token),
            token =>
            {
                var tag = Assert.IsType<SubtitleTagToken>(token);
                Assert.Equal("i", tag.Name);
                Assert.False(tag.IsClosing);
                Assert.Equal(new SubtitleToken(SubtitleTokenType.Text, "world"), Assert.Single(tag.Children));
                Assert.Equal("</i>", tag.ClosingValue);
            },
            token => Assert.Equal(new SubtitleToken(SubtitleTokenType.Text, " "), token),
            token => Assert.Equal(new SubtitleToken(SubtitleTokenType.Info, "[music]"), token));
    }

    [Fact]
    public void Tokenize_NestedTags_BuildsChildTokens()
    {
        var root = Assert.IsType<SubtitleTagToken>(
            Assert.Single(SubtitleTokenizer.Tokenize("<b>bold <i>italic</i></b>")));
        var italic = Assert.IsType<SubtitleTagToken>(root.Children[1]);

        Assert.Equal("b", root.Name);
        Assert.Equal("i", italic.Name);
        Assert.Equal(
            new SubtitleToken(SubtitleTokenType.Text, "italic"),
            Assert.Single(italic.Children));
        Assert.Equal("</i>", italic.ClosingValue);
        Assert.Equal("</b>", root.ClosingValue);
    }

    [Fact]
    public void Tokenize_DeeplyNestedTags_DoesNotRecurse()
    {
        const int depth = 10000;
        var input = string.Concat(Enumerable.Repeat("<b>", depth))
                    + "text"
                    + string.Concat(Enumerable.Repeat("</b>", depth));

        var tokens = SubtitleTokenizer.Tokenize(input);
        SubtitleToken current = Assert.Single(tokens);

        for (var index = 0; index < depth; index++)
        {
            var tag = Assert.IsType<SubtitleTagToken>(current);
            Assert.Equal("b", tag.Name);
            current = Assert.Single(tag.Children);
        }

        Assert.Equal("text", Assert.IsType<SubtitleToken>(current).Value);
    }

    [Fact]
    public void Tokenize_TagWithAttributes_ParsesNameAndAttributeValues()
    {
        var token = Assert.IsType<SubtitleTagToken>(
            Assert.Single(SubtitleTokenizer.Tokenize(
                """<font COLOR="#ff>00" face='Comic Sans MS' size=24/>""")));

        Assert.Equal("font", token.Name);
        Assert.True(token.IsSelfClosing);
        Assert.Equal("#ff>00", token.Attributes["color"]);
        Assert.Equal("Comic Sans MS", token.Attributes["face"]);
        Assert.Equal("24", token.Attributes["size"]);
    }

    [Theory]
    [InlineData("\r\n")]
    [InlineData("\n")]
    [InlineData("\r")]
    public void Tokenize_LineEndings_ReturnNormalizedNewlineToken(string lineEnding)
    {
        var tokens = SubtitleTokenizer.Tokenize($"first{lineEnding}second");

        Assert.Collection(
            tokens,
            token => Assert.Equal(new SubtitleToken(SubtitleTokenType.Text, "first"), token),
            token => Assert.Equal(new SubtitleToken(SubtitleTokenType.NewLine, "\n"), token),
            token => Assert.Equal(new SubtitleToken(SubtitleTokenType.Text, "second"), token));
    }

    [Theory]
    [InlineData("unfinished <tag", "<tag")]
    [InlineData("unfinished [info", "[info")]
    public void Tokenize_UnclosedMarkup_ReturnsRemainingInputAsBadToken(string input, string badToken)
    {
        var tokens = SubtitleTokenizer.Tokenize(input);

        Assert.Collection(
            tokens,
            token => Assert.Equal(new SubtitleToken(SubtitleTokenType.Text, "unfinished "), token),
            token =>
            {
                Assert.Equal(SubtitleTokenType.BadToken, token.Type);
                Assert.Equal(badToken, token.Value);
            });
    }

    [Fact]
    public void Tokenize_NullInput_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => SubtitleTokenizer.Tokenize(null!));
    }
}
