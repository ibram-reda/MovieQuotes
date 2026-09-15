using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MovieQuotes.UI.Models;
using Avalonia;
using Avalonia.Styling;
using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace MovieQuotes.UI.Services;


public sealed class SettingsService : INotifyPropertyChanged
{ 
    private readonly string _settingsFilePath;
    private readonly IConfigurationRoot configuration;

    public event PropertyChangedEventHandler? PropertyChanged;

    public ApplicationSettings Current { get; private set; }

    public SettingsService(string settingsFilePath, IConfigurationRoot configuration)
    { 
        _settingsFilePath =  settingsFilePath;
        this.configuration = configuration;
        Current = Load();
    }

    public async Task SaveAsync( ApplicationSettings settings,
        CancellationToken cancellationToken = default)
    {
        await using var stream = File.Create(_settingsFilePath);

        await JsonSerializer.SerializeAsync(
            stream, settings,
            cancellationToken: cancellationToken);
    }

    private ApplicationSettings Load()
    {
        if (!File.Exists(_settingsFilePath))
            return new ApplicationSettings();

      
        var json = File.ReadAllText(_settingsFilePath);

        return  JsonSerializer.Deserialize<ApplicationSettings>(json)
               ?? new ApplicationSettings();
    }

    internal async Task SetDarkMode(bool value)
    {
        Current.IsDarkMode = value;
        if (App.Current is { } app)
        {
            app.RequestedThemeVariant = value ? ThemeVariant.Dark : ThemeVariant.Light;
        }
        OnPropertyChanged(nameof(Current.IsDarkMode));

        await SaveAsync(Current);
    }

    private void OnPropertyChanged(string? v = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
    }
}