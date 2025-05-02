namespace MovieQuotes.Application.Common.Models;

using MovieQuotes.Application.Common.Enums;

public class OperationResult<T>
{
    private List<Error> errors = new List<Error>();
    public T? Payload { get; set; }

    public bool IsError { get; private set; }
    public bool IsSuccess => !IsError;

    public IEnumerable<Error> Errors => errors;

    public void AddErrorRange(IEnumerable<Error> errors)
    {
        IsError = true;
        this.errors.AddRange(errors);
    }

    public void AddError(ErrorCode code, string message)
    {
        IsError = true;
        errors.Add(new(code, message));
    }

    public void AddError(ErrorCode code, string message, params object[] args)
    {
        var formattedMessage = string.Format(message, args);
        AddError(code, formattedMessage);
    }

    public void AddUnknownError(string message)
    {
        AddError(ErrorCode.UnKnownError, message);
    }
}
