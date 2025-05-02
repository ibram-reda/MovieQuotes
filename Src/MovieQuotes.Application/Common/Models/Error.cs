namespace MovieQuotes.Application.Common.Models;

using MovieQuotes.Application.Common.Enums;

public class Error
{
    public Error(ErrorCode code, string message = "")
    {
        Code = code;
        Message = message;
    }
    public ErrorCode Code { get; }
    public string Message { get; } = string.Empty;
}
