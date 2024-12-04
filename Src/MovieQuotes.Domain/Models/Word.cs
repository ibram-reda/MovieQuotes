namespace MovieQuotes.Domain.Models;

public class Word
{
    private Word() { }
    private Word(string text)
    {
        this.Text = text;
    }
    public int Id { get; }
    public string Text { get; } = string.Empty;

    public virtual List<PhraseWords> Phrases { get; } = [];
    public static Word CreateWord(string text)
    {
        if (text.Contains(" "))
            throw new ArgumentException("Word can not contains Spaces!");
        return new Word(text);
    }
}
