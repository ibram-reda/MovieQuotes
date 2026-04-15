namespace MovieQuotes.UI.Tests.FeatureTests.WatchMovie;

using MovieQuotes.UI.Features.Movies.WatchMovie;


public class SubtitleManagerTests
{
    [Fact]
    public void UpdateTest()
    {
        SubtitleEntry[] subtitles = new SubtitleEntry[]
        {
            new SubtitleEntry(1, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(2), "Hello World"),
            new SubtitleEntry(2, TimeSpan.FromSeconds(4), TimeSpan.FromSeconds(6), "Welcome to MovieQuotes"),
            new SubtitleEntry(3, TimeSpan.FromSeconds(9), TimeSpan.FromSeconds(10), "Enjoy the movie!")
        };
        SubtitleManager manger = new SubtitleManager(subtitles);

        manger.Update(TimeSpan.FromSeconds(1));
        Assert.NotNull(manger.CurrentSubtitle);
        Assert.Equal(1, manger.CurrentSubtitle!.Id);

        manger.Update(TimeSpan.FromMilliseconds(2500));
        Assert.Null(manger.CurrentSubtitle);

        manger.Update(TimeSpan.FromSeconds(3));
        Assert.Null(manger.CurrentSubtitle); 

        manger.Update(TimeSpan.FromSeconds(4));
        Assert.NotNull(manger.CurrentSubtitle);
        Assert.Equal(2, manger.CurrentSubtitle!.Id);


        manger.Update(TimeSpan.FromSeconds(1));
        Assert.NotNull(manger.CurrentSubtitle);
        Assert.Equal(1, manger.CurrentSubtitle!.Id);

        manger.Update(TimeSpan.FromSeconds(7));
        Assert.Null(manger.CurrentSubtitle); 

        manger.Update(TimeSpan.FromSeconds(9));
        Assert.NotNull(manger.CurrentSubtitle);
        Assert.Equal(3, manger.CurrentSubtitle!.Id);

    }


    [Fact]
    public void PreviousTest()
    {
        SubtitleEntry[] subtitles = new SubtitleEntry[]
        {
            new SubtitleEntry(1, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(2), "Hello World"),
            new SubtitleEntry(2, TimeSpan.FromSeconds(4), TimeSpan.FromSeconds(6), "Welcome to MovieQuotes"),
            new SubtitleEntry(3, TimeSpan.FromSeconds(9), TimeSpan.FromSeconds(10), "Enjoy the movie!")
        };
        SubtitleManager manger = new SubtitleManager(subtitles);
        var phrase = manger.GetPreviousPhrase(TimeSpan.FromSeconds(3));
        Assert.NotNull(phrase);
        Assert.Equal(1, phrase.Id);

        phrase = manger.GetPreviousPhrase(TimeSpan.FromSeconds(7));
        Assert.NotNull(phrase);
        Assert.Equal(2, phrase.Id);

        phrase = manger.GetPreviousPhrase(TimeSpan.FromSeconds(11));
        Assert.NotNull(phrase);
        Assert.Equal(3, phrase.Id);

        phrase = manger.GetPreviousPhrase(TimeSpan.FromSeconds(5));
        Assert.NotNull(phrase);
        Assert.Equal(1, phrase.Id);
    }

    [Fact]
    public void NextTest()
    {
        SubtitleEntry[] subtitles = new SubtitleEntry[]
        {
            new SubtitleEntry(1, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(2), "Hello World"),
            new SubtitleEntry(2, TimeSpan.FromSeconds(4), TimeSpan.FromSeconds(6), "Welcome to MovieQuotes"),
            new SubtitleEntry(3, TimeSpan.FromSeconds(9), TimeSpan.FromSeconds(10), "Enjoy the movie!")
        };
        SubtitleManager manger = new SubtitleManager(subtitles);
        var phrase = manger.GetNextPhrase(TimeSpan.FromSeconds(3));
        Assert.NotNull(phrase);
        Assert.Equal(2, phrase.Id);

        phrase = manger.GetNextPhrase(TimeSpan.FromSeconds(7));
        Assert.NotNull(phrase);
        Assert.Equal(3, phrase.Id);

        phrase = manger.GetNextPhrase(TimeSpan.FromSeconds(-1));
        Assert.NotNull(phrase);
        Assert.Equal(1, phrase.Id);

        phrase = manger.GetNextPhrase(TimeSpan.FromSeconds(10));
        Assert.Null(phrase);
    }

}
