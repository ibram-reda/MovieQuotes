namespace MovieQuotes.Application.Models;

public class OperationPageResult<T> : OperationResult<List<T>>    
{
    public int Count { get; set; }
    public bool HasNext { get; set; }
    public uint CurrentPageNumber { get; set; }
    public uint ItemPerPage { get; set; }
}
