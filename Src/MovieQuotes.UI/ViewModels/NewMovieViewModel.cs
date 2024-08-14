namespace MovieQuotes.UI.ViewModels;


using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieQuotes.Application.Operations.Commands;
using MovieQuotes.UI.Models;
using MovieQuotes.UI.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Xabe.FFmpeg;


internal partial class NewMovieViewModel : ViewModelBase
{
    private readonly IFilesService filesService;
    [ObservableProperty] private List<SubtitlePhrase>? _moviePhrases = new();
    [ObservableProperty] private ObservableCollection<SubtitlePhrase> _processedMoviePhrases = new();
    [ObservableProperty] private string _movieVideoPath = string.Empty;
    [ObservableProperty] private string _movieName = string.Empty;
    [ObservableProperty] private string _movieSubtitlePath = string.Empty;
    [ObservableProperty] private bool _isProcessing = false;


    public NewMovieViewModel()
    {
        this.filesService = this.GetService<IFilesService>();

    }

    [RelayCommand]
    private async Task LoadSubtitle(CancellationToken token)
    {
        ErrorMessages?.Clear();
        try
        {
            var file = await filesService.OpenFileAsync("select subtitleFolder");
            if (file is null) return;
            MovieSubtitlePath = file.Path.LocalPath;

            await using var readStream = await file.OpenReadAsync();
            using var reader = new StreamReader(readStream);

            MoviePhrases = await SubtitlePhrase.GetPhrasesFromStreamAsync(reader);

        }
        catch (Exception e)
        {
            ErrorMessages?.Add(e.Message);
        }
    }

    [RelayCommand]
    private async Task LoadVideo(CancellationToken token)
    {
        var file = await filesService.OpenFileAsync("open movie file");
        if (file is null) return;
        MovieVideoPath = file.Path.LocalPath;
        MovieName = file.Name.Remove(file.Name.LastIndexOf('.'));
    }


    [RelayCommand(IncludeCancelCommand = true)]
    private async Task StartProcessAsync(CancellationToken token = default)
    {
        ErrorMessages?.Clear();
        if (string.IsNullOrEmpty(MovieVideoPath))
        {
            ErrorMessages?.Add("Movie Path is not valid!");
            return;
        }
        if (MoviePhrases?.Count == ProcessedMoviePhrases.Count)
        {
            ErrorMessages?.Add("Nothing Found to process");
            return;
        }

        var mediaInfo = await FFmpeg.GetMediaInfo(MovieVideoPath, token);
        var videoStream = mediaInfo.VideoStreams.First();
        var audioStream = mediaInfo.AudioStreams.First();

        LoadExistingClipsInfo();

        IsProcessing = true;
        foreach (var phrase in MoviePhrases?.Except(ProcessedMoviePhrases) ?? [])
        {
            string outputFileName = $"{MovieName}/{phrase.Sequence}.MP4";

            await FFmpeg.Conversions.New()
                .AddStream(videoStream.Split(phrase.StartTime, phrase.Duration))
                .AddStream(audioStream.Split(phrase.StartTime, phrase.Duration))
                .SetOutput(outputFileName)
                .Start();

            ProcessedMoviePhrases.Add(phrase);

            if (token.IsCancellationRequested)
            {
                break;
            }
        }

        IsProcessing = false;

    }

    [RelayCommand(IncludeCancelCommand = true)]
    private async Task SaveIntoDb(CancellationToken token = default)
    {

        var command = new CreateMovieCommand(MovieName, MovieVideoPath, MovieSubtitlePath);

        var result = await mediator.Send(command);

        if (result.IsError)
        {
            foreach (var error in result.Errors)
                ErrorMessages?.Add(error.Message);
            return;
        }

        MovieVideoPath = string.Empty;
        MovieSubtitlePath = string.Empty;
    }

    [RelayCommand]
    private void BackToWelcomeScreen()
    {
        this.NavigationService.NavigateTo<WelcomeScreenViewModel>();
    }

    private void LoadExistingClipsInfo()
    {
        if (!Directory.Exists(MovieName)) return;

        var ExistingSquances = Directory.GetFiles(MovieName)
            .Select(a => Path.GetFileNameWithoutExtension(a))
            .Where(a => Regex.IsMatch(a, @"^\d+$"))
            .Select(a => int.Parse(a))
            .Except(ProcessedMoviePhrases.Select(p => p.Sequence))
            .OrderBy(a => a);

        foreach (var sequance in ExistingSquances)
        {
            var phrase = MoviePhrases?.FirstOrDefault(a => a.Sequence == sequance);
            if (phrase is not null)
                ProcessedMoviePhrases.Add(phrase);
        }

    }
}
