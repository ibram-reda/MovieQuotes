namespace MovieQuotes.Domain.Tests;

using MovieQuotes.Domain.Models;

public class SubtitlePhraseTests
{
    [Fact]
    public void ParsTest()
    {
        string strBlock = """
            2
            00:00:17,894 --> 00:00:21,189
            No matter what they say,
            it's all about money.
            """;

        string txtContent = """
            No matter what they say, it's all about money.
            """;
        int sequence = 2;
        TimeSpan startTime = TimeSpan.Parse("00:00:17.894");
        TimeSpan endTime = TimeSpan.Parse("00:00:21.189");

        var result = SubtitlePhrase.Parse(strBlock);

        Assert.Equal(sequence, result.Sequence);
        Assert.Equal(startTime, result.StartTime);
        Assert.Equal(endTime, result.EndTime);
        Assert.Equal(txtContent, result.Text);
    }

    [Fact]
    public void RemoveMarkup_ItalicTags()
    {
        string strBlock = """
            464
            00:34:24,775 --> 00:34:28,236
            <i>Edward, I know a lot of nice girls.</i>
            No, you don't.
            """;

        string txtContent = """
            Edward, I know a lot of nice girls. No, you don't.
            """;
        int sequence = 464;
        TimeSpan startTime = TimeSpan.Parse("00:34:24.775");
        TimeSpan endTime = TimeSpan.Parse("00:34:28.236");

        var result = SubtitlePhrase.Parse(strBlock);

        Assert.Equal(sequence, result.Sequence);
        Assert.Equal(startTime, result.StartTime);
        Assert.Equal(endTime, result.EndTime);
        Assert.Equal(txtContent, result.Text);
    }

    [Fact]
    public void RemoveMarkup_BoldTags()
    {
        string strBlock = """
            464
            00:34:24,775 --> 00:34:28,236
            Well,<b> you keep saying the future wasn't always this way, right?</b>
            """;

        string txtContent = """
            Well, you keep saying the future wasn't always this way, right?
            """;

        var result = SubtitlePhrase.Parse(strBlock);

        Assert.Equal(txtContent, result.Text);
    }

    [Fact]
    public void RemoveMarkup_UnderlineTags() {
         string strBlock = """
            464
            00:34:24,775 --> 00:34:28,236
            Well,<u> you keep saying the future wasn't always this way, right?</u>
            """;

        string txtContent = """
            Well, you keep saying the future wasn't always this way, right?
            """;

        var result = SubtitlePhrase.Parse(strBlock);

        Assert.Equal(txtContent, result.Text);
    }

    [Fact]
    public void RemoveMarkup_FontColorTags()
    {
        string strBlock = """
            464
            00:34:24,775 --> 00:34:28,236
            Well,<font color="#FFFFFF"> you keep saying the future wasn't always this way, right?</font>
            """;

        string txtContent = """
            Well, you keep saying the future wasn't always this way, right?
            """;

        var result = SubtitlePhrase.Parse(strBlock);
         
        Assert.Equal(txtContent, result.Text);
    }

    [Fact]
    public void RemoveMarkup_Hyphen()
    {
        string strBlock = """
            464
            00:34:24,775 --> 00:34:28,236
            - How's it going?
            - Oh, good.
            """;

        string txtContent = """
            How's it going? Oh, good.
            """;

        var result = SubtitlePhrase.Parse(strBlock);

        Assert.Equal(txtContent, result.Text);
    }

    [Fact]
    public void NormalizeText()
    {
        string Text = """ 
                 <b>Hello</b>
            can
                 - Oh, good.
            <i>a
            </i>  

            """;

        string expected = """
            Hello can Oh, good. a
            """;

        var result = SubtitlePhrase.NormalizeText(Text);

        Assert.Equal(expected, result);
    }
}
