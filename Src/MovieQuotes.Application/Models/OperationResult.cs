namespace MovieQuotes.Application.Models;

using MovieQuotes.Application.Enums;

public class OperationResult<T>
{
    private List<Error> errors = new List<Error>();
    public T? Payload { get; set; }

    public bool IsError { get; private set; }
    public bool IsSuccess => !IsError;

    public IEnumerable<Error> Errors => errors;


    public void AddError(ErrorCode code, string message)
    {
        IsError = true;
        this.errors.Add(new(code, message));
    }

    public void AddError(ErrorCode code, string message, params object[] args)
    {
        var formattedMessage = string.Format(message, args);
        AddError(code, formattedMessage);
    }

    public void AddUnknownError(string message)
    {
        this.AddError(ErrorCode.UnKnownError, message);
    }
}
