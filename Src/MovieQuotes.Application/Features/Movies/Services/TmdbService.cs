namespace MovieQuotes.Application.Features.Movies.Services;

using MovieQuotes.Domain.Models;
public class TmdbService
{
    private readonly string _apiKey;
    private readonly HttpClient _httpClient;

    public TmdbService(string apiKey)
    {
        _apiKey = apiKey;
        _httpClient = new HttpClient();
    }

    // Methods to interact with TMDb API would go here
    /// <summary>
    /// Fetches TMDb data for a given movie using its IMDBId. If the data has already been fetched and Cashed to a file,
    /// it reads from the cashed file instead of making an API call. 
    /// This helps to avoid unnecessary API calls and can improve performance.
    /// </summary>
    /// <param name="movie">the movie for which to fetch TMDb data</param>
    /// <returns>json string containing the TMDb data</returns>
    /// <exception cref="InvalidOperationException">if the API key or IMDBId is not provided</exception>
    public async Task<string?> FetchTmdbDataAsync(Movie movie)
    {
        
        var TMDBInfoPath = Path.Combine(movie.BaseFolderDir, movie.FolderName, $"TMDB-info.json");
        if(File.Exists(TMDBInfoPath))
        {
            return await File.ReadAllTextAsync(TMDBInfoPath); // already has Call TMDB info file before, so return the cashed content of the file to avoid unnecessary API calls
        }
        if (string.IsNullOrEmpty(_apiKey))
        {
            throw new InvalidOperationException("TMDB API key is required to aquire Data from TMDb");
        }
        if (string.IsNullOrEmpty(movie.IMDBId))
        {
            throw new InvalidOperationException("IMDBId is required to aquire Data from TMDb");
        }
 
        var url =
        $"https://api.themoviedb.org/3/find/{movie.IMDBId}?api_key={_apiKey}&external_source=imdb_id";

        var json = await _httpClient.GetStringAsync(url);

        // Save the TMDB info to a file for future reference
        // Cashing the TMDB info can help avoid unnecessary API calls in the future, especially if the backdrop information is already available.
        await File.WriteAllTextAsync(TMDBInfoPath, json);

        return json;
    }

    public async Task<List<int>> GetMovieGenresAsync(Movie movie)
    {
        var json = await this.FetchTmdbDataAsync(movie);
        // Deserialize the JSON and extract the genre IDs
        using var doc = System.Text.Json.JsonDocument.Parse(json);

        var results = doc.RootElement.GetProperty("movie_results");
        if (results.GetArrayLength() == 0)
        {
            Console.WriteLine($"Data not found for movie: {movie.IMDBId}");
            return [];
        }

        var genres= results[0].GetProperty("genre_ids").EnumerateArray().Select(g => g.GetInt32()).ToList();  
        return genres;
    }


}