namespace MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Common.Enums;
public class OperationPageResult<T> : OperationResult<List<T>>
{
    public int Count { get; set; }
    public bool HasNext { get; set; }
    public uint CurrentPageNumber { get; set; }
    public uint ItemPerPage { get; set; }

    public static OperationPageResult<T> Success(List<T> payload)
    {
        return new OperationPageResult<T>
        {
            Payload = payload,
            Count = payload.Count,
            HasNext = false,
            CurrentPageNumber = 1,
            ItemPerPage = (uint)payload.Count
        };
    }
    public static OperationPageResult<T> Failure(ErrorCode code, string message)
    {
        var result = new OperationPageResult<T>();
        result.AddError(code, message);
        return result;
    }
}
