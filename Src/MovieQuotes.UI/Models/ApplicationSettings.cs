using System;
using System.IO;
using MovieQuotes.Application.Common.Models;

namespace MovieQuotes.UI.Models;


public sealed class ApplicationSettings 
{
    // User preferences
    public string MoviesLibraryPath { get; set; } = string.Empty;
    public string VideoCacheFolderPath { get; set; } = string.Empty;
    public int SubtitleFontSize { get; set; } = 24;
    public bool IsDarkMode { get; set; } = true;

    public string AppDataFolderPath { get; set; } 

    // User-provided configuration
    public string TmdbApiKey { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;

    // LLM Configuration
    public string OllamaApiUrl { get; set; } = string.Empty;
    public string OllamaModelName { get; set; } = string.Empty;

    public ApplicationSettings()
    {
        AppDataFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MovieQuotes");
    }

    public AppSettings ToAppSettings()
    {
        return new AppSettings
        {
            TmdbApiKey = this.TmdbApiKey,
            ConnectionString = this.ConnectionString,
            OllamaApiUrl = this.OllamaApiUrl,
            OllamaModelName = this.OllamaModelName,
            VideoCashPath = this.VideoCacheFolderPath,
        };
    }
}