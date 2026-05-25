namespace MovieQuotes.Domain.Models;


public class Genre
{
    private Genre() { }
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public List<Movie> Movies { get; } = new();

    public static Genre CreateGenre(int id, string name)
    {
        return new Genre()
        {
            Id = id,
            Name = name,
        };
    }
}