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

    public void AddException(Exception ex)
    {
        var innerExeption = ex;
        var limit = 5; // To prevent potential infinite loops in case of circular references
        while (innerExeption != null)
        {
            AddError(ErrorCode.UnKnownError, innerExeption.Message);
            innerExeption = innerExeption.InnerException;
            if (--limit <= 0)
            {
                AddError(ErrorCode.UnKnownError, "Reached maximum inner exception depth. Possible circular reference.");
                break;
            }
        }
    }

    /// <summary>
    /// Creates a successful operation result with the provided payload.
    /// </summary>
    /// <param name="payload"></param>
    /// <returns></returns>
    internal static OperationResult<T> Success(T payload)
    {
        return new OperationResult<T> { Payload = payload };
    }

    /// <summary>
    /// Creates a failed operation result with the provided errors.
    /// </summary>
    /// <param name="errors"></param>
    /// <returns></returns>
    internal static OperationResult<T> Failure(params Error[] errors)
    {
        var result = new OperationResult<T>();
        result.AddErrorRange(errors);
        return result;
    }

    internal static OperationResult<T> Failure(string error)
    {
        var result = new OperationResult<T>();
        result.AddUnknownError(error);
        return result;
    }


    /// <summary>
    /// Creates a failed operation result with a single error specified by the code and message.
    /// </summary>
    /// <param name="code"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static OperationResult<T> Failure(ErrorCode code, string message)
    {
        var result = new OperationResult<T>();
        result.AddError(code, message);
        return result;
    }

}
