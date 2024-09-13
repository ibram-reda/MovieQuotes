namespace MovieQuotes.Application.Models;

public class Phrase
{
    public int Id { get; set; }
    public int Sequence { get; set; }
    public string Text { get; set; } = string.Empty;
    public string MovieName { get; set; } = string.Empty;
    public string MoviePath { get; set; } = string.Empty;

    public string VideoLocation { get; set; } = string.Empty;

    public TimeSpan StartTime {  get; set; }    
    public TimeSpan EndTime { get; set; }

    public TimeSpan Duration { get; set; }

}
