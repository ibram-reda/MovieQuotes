namespace MovieQuotes.Application.Tests.Features.Movies.SerchMovies;
 
using MovieQuotes.Application.Features.Movies.Queries;


public class MovieSearchParserTests
{
    [Fact]
    public void Parse_WithValidSearch_ReturnsCorrectFilters()
    {
        // Arrange
        var search = "imdb:tt1234567 genre:Action year>=2000 rating>7.5 free text";

        // Act
        var filters = MovieSearchParser.Parse(search);

        // Assert
        Assert.Equal("tt1234567", filters.ImdbId);
        Assert.Equal("Action", filters.Genre);
        Assert.Equal(2000, filters.MinYear);
        Assert.Null(filters.MaxYear);
        Assert.Equal(7.5, filters.MinRating);
        Assert.Null(filters.MaxRating);
        Assert.Equal("free text", filters.Text);
    }

    [Fact]
    public void Parse_WithEmptySearch_ReturnsEmptyFilters()
    {
        // Arrange
        var search = "";
        // Act
        var filters = MovieSearchParser.Parse(search);
        // Assert
        Assert.Null(filters.ImdbId);
        Assert.Null(filters.Genre);
        Assert.Null(filters.MinYear);
        Assert.Null(filters.MaxYear);
        Assert.Null(filters.MinRating);
        Assert.Null(filters.MaxRating);
        Assert.Null(filters.Text);
    }

    [Fact]
    public void Parse_FreeTextOnly_ReturnsEmptyFilters()
    {
        // Arrange
        var search = "free search string";

        // Act
        var filters = MovieSearchParser.Parse(search);

        // Assert
        Assert.Null(filters.ImdbId);
        Assert.Null(filters.Genre);
        Assert.Null(filters.MinYear);
        Assert.Null(filters.MaxYear);
        Assert.Null(filters.MinRating);
        Assert.Null(filters.MaxRating);
        Assert.Equal("free search string", filters.Text);
    }
}