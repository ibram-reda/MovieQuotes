namespace MovieQuotes.Domain.Tests;

using MovieQuotes.Domain.Models;

public class SubtitlePhraseTests
{

    [Theory]
    [InlineData("Hello world", "Hello world", "no changes needed")]
    [InlineData("Hello   world", "Hello world", "should remove extra spaces")]
    [InlineData("  Hello   world  ", "Hello world", "should remove extra spaces and trim it")]
    [InlineData("Hello <i>world</i>", "Hello world", "should remove Italic Tag")]
    [InlineData("Hello <b>world</b>", "Hello world", "should remove Bold Tag")]
    [InlineData("Hello <u>world</u>", "Hello world", "should remove Underline Tag")]
    [InlineData("Hello <font color=\"red\">world</font>", "Hello world", "should remove Font Color Tag")]
    [InlineData("Hello <i>world</i> <b>again</b>", "Hello world again", "should remove multiple markup tags")]
    [InlineData("Hello <i>world</i> <b>again</b>  <u>and more</u>", "Hello world again and more", "should remove multiple markup tags and extra spaces")]
    [InlineData("Hello <i>world</i> <b>again</b>  <u>and more</u>   ", "Hello world again and more", "should remove multiple markup tags, extra spaces and trim it")]
    [InlineData("Hello <i>world</i> <b>again</b>  <u>and more</u>   <font color=\"blue\">with color</font>", "Hello world again and more with color", "should remove multiple markup tags, extra spaces, trim it and keep color tag")]
    [InlineData("Hello <i>world</i> <b>again</b>  <u>and more</u>   <font color=\"blue\">with color</font>   ", "Hello world again and more with color", "should remove multiple markup tags, extra spaces, trim it and keep color tag")]
    public void RemoveMarkupAndDuplicateSpaces(string originalText, string assertText, string msg)
    {
        var phrase = SubtitlePhrase.CreateSubtitlePhrase(1, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), originalText);
        var actualText = phrase.GetTextWithoutMarkupAndDuplicateSpaces();
        Assert.Equal(assertText, actualText);
    }

    [Fact]
    public void RemoveMarkup_StartingHyphen()
    {
        var originalText = "- Hello world";
        var assertText = "Hello world";
        var phrase = SubtitlePhrase.CreateSubtitlePhrase(1, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), originalText);
        var actualText = phrase.GetTextWithoutMarkupAndDuplicateSpaces();
        Assert.Equal(assertText, actualText);
    }


    [Fact]
    public void RemoveMarkup_KeepsHyphenInBetweenWords()
    {
        var originalText = "Hello-world";
        var assertText = "Hello-world";
        var phrase = SubtitlePhrase.CreateSubtitlePhrase(1, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), originalText);
        var actualText = phrase.GetTextWithoutMarkupAndDuplicateSpaces();
        Assert.Equal(assertText, actualText);
    }


    [Fact]
    public void GetWords()
    {
        var originalText = "Hello world";
        var expectedWords = new[] { "Hello", "world" };
        var phrase = SubtitlePhrase.CreateSubtitlePhrase(1, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), originalText);
        var actualWords = phrase.GetWords(); 
        Assert.Equal(expectedWords, actualWords);

    }

    [Fact]
    public void GetWords_WithMarkup()
    {
        var originalText = "Hello <i>world</i>";
        var expectedWords = new[] { "Hello", "world" };
        var phrase = SubtitlePhrase.CreateSubtitlePhrase(1, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), originalText);
        var actualWords = phrase.GetWords();
        Assert.Equal(expectedWords, actualWords);
    }

    [Fact]
    public void GetWords_WithMultipleSpaces()
    {
        var originalText = "Hello   world";
        var expectedWords = new[] { "Hello", "world" };
        var phrase = SubtitlePhrase.CreateSubtitlePhrase(1, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), originalText);
        var actualWords = phrase.GetWords();
        Assert.Equal(expectedWords, actualWords);
    }

    [Fact]
    public void GetWords_EmptyText_ReturnsEmptyArray()
    {
        var originalText = "";
        var expectedWords = Array.Empty<string>();
        var phrase = SubtitlePhrase.CreateSubtitlePhrase(1, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), originalText);
        var actualWords = phrase.GetWords();
        Assert.Equal(expectedWords, actualWords);
    }


}
