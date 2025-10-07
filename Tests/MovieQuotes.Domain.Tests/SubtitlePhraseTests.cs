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
    [InlineData("what it is. </ i>", "what it is.", "should remove closing tag with space")]
    [InlineData("Not a soul! </ I>", "Not a soul!", "should remove closing tag with space")]
    [InlineData("<font face=\"Comic Sans MS\" color=\"#ffff80\">MEDUSA: Warning.", "MEDUSA: Warning.", "should remove font with face")]
    [InlineData("<font color=\"#ffff80\" face=\"Comic Sans MS\">MEDUSA: Warning.", "MEDUSA: Warning.", "should remove font with face")]
    [InlineData("<font >MEDUSA: Warning.", "MEDUSA: Warning.", "should remove font with face")]
    public void RemoveMarkupAndDuplicateSpaces(string originalText, string assertText, string msg)
    {
        var phrase = SubtitlePhrase.CreateSubtitlePhrase(1, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), originalText);
        var actualText = phrase.GetTextWithoutMarkupAndDuplicateSpaces();
        Assert.Equal(assertText, actualText);
    }

    [Fact]
    public void RemoveMarkup_KeepStartingHyphen()
    {
        var originalText = "- Hello world";
        var assertText = "- Hello world";
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

    [Theory]
    [InlineData("Hello,world", "Hello", "world")]
    [InlineData("No,no,no,no", "No", "no", "no", "no")]
    [InlineData("right?Mmhmm", "right", "Mmhmm")]
    [InlineData("Hello, world! How are you?", "Hello", "world", "How", "are", "you")]
    [InlineData("I'm on Cheapapartments.com,", "I'm", "on", "Cheapapartments.com")]
    [InlineData("Monica. No! I have to....", "Monica", "No", "I", "have", "to")]
    [InlineData("I don't know, I just... don't know.", "I", "don't", "know", "I", "just", "don't", "know")]
    [InlineData("And when they left, he came again... Calvera...and every year since.", "And", "when", "they", "left", "he", "came", "again", "Calvera", "and", "every", "year", "since")]
    [InlineData("Ellen...\"What?\"", "Ellen", "What")]
    [InlineData("Jabberwock12.listserv@harvard.edu", "Jabberwock12.listserv@harvard.edu")]
    [InlineData("One day, I woke up stupid.You did?", "One", "day", "I", "woke", "up", "stupid", "You", "did")]
    [InlineData("5...4...3...2...1", "5", "4", "3", "2", "1")]
    [InlineData("in the 1,000year history of our kingdom.", "in", "the", "1,000", "year", "history", "of", "our", "kingdom")]
    [InlineData("I'm gonna give 'em \r\na super-duper fuckin' dose.", "I'm","gonna","give","'em","a" )]
    [InlineData("Bet it's super-dumb.","Bet","it's", "super-dumb")]
    public void GetWords_Separate(string originalText,params string[] expectedWords)
    { 
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
