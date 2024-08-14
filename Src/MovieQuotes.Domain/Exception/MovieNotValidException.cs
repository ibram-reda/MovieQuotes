namespace MovieQuotes.Domain.Exception;

public class MovieNotValidException : NotValidException
{
    internal MovieNotValidException()
    {
    }

    internal MovieNotValidException(string? message) : base(message)
    {
    }

    internal MovieNotValidException(string? message, System.Exception? innerException) : base(message, innerException)
    {
    }
}
