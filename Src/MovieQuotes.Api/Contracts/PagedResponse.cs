namespace MovieQuotes.Api.Contracts;

public class PagedResponse<T> : BaseResponse<List<T>>
{
    public int Count { get; set; }
    public bool HasNext { get; set; }
    public int CurrentPageNumber { get; set; }
    public int ItemPerPage { get; set; }
}