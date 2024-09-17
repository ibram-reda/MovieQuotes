namespace MovieQuotes.Api.Contracts;

public class BaseResponse <T>
{
    public bool IsSuccess { get; set; }
    public T? PayLoad { get; set; }

    public List<string> Errors { get; set; } = [];
    public DateTime Timestamp { get; set; } = DateTime.Now;
}
