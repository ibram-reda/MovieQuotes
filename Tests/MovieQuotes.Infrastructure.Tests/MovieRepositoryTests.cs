namespace MovieQuotes.Infrastructure.Tests;

using MovieQuotes.Infrastructure.Repositories;
public class MovieRepositoryTests : IClassFixture<DbFixture>
{
    private readonly DbFixture _fixture;

    public MovieRepositoryTests(DbFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetAll_ReturnsAllMovies()
    {
        // Arrange
        var repository = new MovieRepository(_fixture.Context);

        // Act
        var movies = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(movies);
        Assert.Equal(3, movies.Count());
    }

    [Fact]
    public async Task GetById_ReturnsCorrectMovie()
    {
        // Arrange
        var repository = new MovieRepository(_fixture.Context);
        var existingMovie = _fixture.Context.Movies.First();

        // Act
        var movie = await repository.GetByIdAsync(existingMovie.Id);
        // Assert
        Assert.NotNull(movie);
        Assert.Equal(existingMovie.Id, movie.Id);
        Assert.Equal(existingMovie.Title, movie.Title);       

    }
}