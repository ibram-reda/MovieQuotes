namespace MovieQuotes.UI.Tests.ModelsTests;

using MovieQuotes.UI.Models;
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
            No matter what they say,
            it's all about money.
            """;
        int sequence = 2;
        TimeSpan startTime = TimeSpan.Parse("00:00:17.894");
        TimeSpan endTime = TimeSpan.Parse("00:00:21.189");

        var result = SubtitlePhrase.Parse(strBlock);

        Assert.Equal(sequence, result.Sequence);
        Assert.Equal(startTime,result.StartTime);
        Assert.Equal(endTime,result.EndTime);
        Assert.Equal(txtContent,result.Text);
    }
}
