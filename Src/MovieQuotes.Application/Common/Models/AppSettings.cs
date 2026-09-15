namespace MovieQuotes.Application.Common.Models;


public class AppSettings
{
    public string TmdbApiKey { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;

    // LLM Configuration
    public string OllamaApiUrl { get; set; } = string.Empty;
    public string OllamaModelName { get; set; } = string.Empty;
}