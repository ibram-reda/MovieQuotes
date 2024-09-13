namespace MovieQuotes.Api.Contracts.Responses;

public class PhraseResponse
{
    public int Id { get; set; }
    public int Sequence { get; set; }
    public string Text { get; set; } = string.Empty;
    public string MovieName { get; set; } = string.Empty;     
    public string VideoUrl { get; set; } = string.Empty;

    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }     

}
