namespace MovieQuotes.UI.ViewModels;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieQuotes.UI.Models;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private IEnumerable? _moviePhrases;

    [RelayCommand]
    private async Task LoadSubtitle(CancellationToken token)
    {
        ErrorMessages?.Clear();
        try
        {
            var file = await DoOpenFilePickerAsync();
            if (file is null) return;

            var fileInfo = await file.GetBasicPropertiesAsync();
            // Limit the text file to 3MB.
            if (fileInfo.Size <= 1024 * 1024 * 3)
            {
                await using var readStream = await file.OpenReadAsync();
                using var reader = new StreamReader(readStream);

                var FileTextContent = await reader.ReadToEndAsync(token);
                var values = SubtitlePhrase.SubtitleBlockRegex
                    .Matches(FileTextContent);
                MoviePhrases = values.Select(a => SubtitlePhrase.Parse(a));
            }
            else
            {
                throw new Exception("File exceeded 3MB limit.");
            }
        }
        catch (Exception e)
        {
            ErrorMessages?.Add(e.Message);
        }
    }



    private async Task<IStorageFile?> DoOpenFilePickerAsync()
    {
        // TODO: you should follow the MVVM principles
        // by making service classes and locating them with DI/IoC.
        // instead of directly get the reference
        // for StorageProvider APIs here inside the ViewModel. 

        // See IoCFileOps project for an example of how to accomplish this.
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
            desktop.MainWindow?.StorageProvider is not { } provider)
            throw new NullReferenceException("Missing StorageProvider instance.");

        var files = await provider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = "Select subtitle File",
            AllowMultiple = false
        });

        return files?.Count >= 1 ? files[0] : null;
    }

}