namespace MovieQuotes.Application.Models;

using MovieQuotes.Application.Enums;

public class Error
{
    public Error(ErrorCode code, string message = "")
    {
        this.Code = code;
        this.Message = message;
    }
    public ErrorCode Code { get; }
    public string Message { get; } = string.Empty;
}
