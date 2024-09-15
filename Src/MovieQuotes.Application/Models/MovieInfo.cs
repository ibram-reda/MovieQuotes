
using MovieQuotes.Domain.Models;

namespace MovieQuotes.Application.Models;

public class MovieInfo
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string LocalPath { get; set; } = string.Empty;
    public string? IMDBId { get; set; } = string.Empty;

    public string? CoverUrl {  get; set; } = string.Empty;
}