namespace MovieQuotes.UI.ViewModels;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieQuotes.UI.Models;
using MovieQuotes.UI.Services;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IFilesService filesService;
    [ObservableProperty] private IEnumerable? _moviePhrases;

    public MainWindowViewModel(IFilesService filesService)
    {
        this.filesService = filesService;
    }    

    [RelayCommand]
    private async Task LoadSubtitle(CancellationToken token)
    {
        ErrorMessages?.Clear();
        try
        {
            var file = await filesService.OpenFileAsync();
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

         
}