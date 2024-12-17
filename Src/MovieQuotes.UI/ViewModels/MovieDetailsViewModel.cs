namespace MovieQuotes.UI.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibVLCSharp.Shared;
using MovieQuotes.Application.Models;
using MovieQuotes.Application.Operations.Commands;
using MovieQuotes.Application.Operations.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


internal partial class MovieDetailsViewModel : ViewModelBase, IDisposable
{
    private LibVLC MainLibVLC { get; }
    public MediaPlayer MainMediaPlayer { get; }

    public MovieDetailsViewModel()
    {
        MainLibVLC = new();
        MainMediaPlayer = new(MainLibVLC);
        MainMediaPlayer.TimeChanged += MainMediaPlayer_TimeChanged;
        MainMediaPlayer.LengthChanged += MainMediaPlayer_LengthChanged;
        MainMediaPlayer.PositionChanged += MainMediaPlayer_PositionChanged;

    }

    private void MainMediaPlayer_PositionChanged(object? sender, MediaPlayerPositionChangedEventArgs e)
    {
        var value = (long)(e.Position * MovieLength);
        this.SetProperty(ref this.currentTime, value, nameof(CurrentTime));
    }

    private void MainMediaPlayer_LengthChanged(object? sender, MediaPlayerLengthChangedEventArgs e)
    {
        this.MovieLength = e.Length;
    }

    private void MainMediaPlayer_TimeChanged(object? sender, MediaPlayerTimeChangedEventArgs e)
    {
        this.SetProperty(ref this.currentTime, e.Time, nameof(CurrentTime));
        this.UpdateCurrentPhrase();
    }

    public override string Title => $"Details of {MovieName}";

    [ObservableProperty] int _id;
    [NotifyPropertyChangedFor(nameof(Title))]
    [ObservableProperty] string movieName = "";
    [ObservableProperty] string videoLocation = "";
    [ObservableProperty] long movieLength = 0;
    [ObservableProperty] private bool viewContent;
    [ObservableProperty] List<Phrase> phrases = [];
    [ObservableProperty] Phrase? currentPhrase;
    [ObservableProperty] string learningContent = "";
    [ObservableProperty] string translateContent = "";
    [ObservableProperty] string contentType = "";
    public string[] AllowedType { get; } = ["noun", "adjective", "verb", "idiom", "phrasal verb",];

    private long currentTime = 0;

    public long CurrentTime
    {
        get => this.currentTime;
        set
        {
            MainMediaPlayer.Time = value;
        }
    }

    public override async Task InitAsync(object? message)
    {
        if (message is int movieId)
            await LoadDataAsync(movieId);
    }

    async Task LoadDataAsync(int movieId)
    {
        var query = new GetMovieDetailsQuery { MovieId = movieId };

        var reuslt = await this.mediator.Send(query);

        if (reuslt.IsError)
            return;

        this.Id = reuslt.Payload!.Id;
        this.MovieName = reuslt.Payload.Title;
        this.VideoLocation = reuslt.Payload.LocalPath;
        this.Phrases = reuslt.Payload.phrases;



        var uri = new Uri(VideoLocation);
        Media media = new Media(this.MainLibVLC, uri);
        MainMediaPlayer.Media = media;
    }

    public void Dispose()
    {
        this.MainMediaPlayer.Stop();
        this.MainMediaPlayer.Dispose();
        this.MainLibVLC.Dispose();
    }

    void UpdateCurrentPhrase()
    {
        var ctime = TimeSpan.FromMilliseconds(CurrentTime);
        this.CurrentPhrase =
            this.phrases?.FirstOrDefault(a => a.StartTime <= ctime && ctime <= a.EndTime);

    }

    [RelayCommand]
    async Task OpenPopUp()
    {
        if (this.MainMediaPlayer.IsPlaying)
            this.MainMediaPlayer.Pause();
        this.ViewContent = true;
        await Task.Delay(300);
    }

    [RelayCommand]
    void ClosePopUp()
    {
        if (!this.MainMediaPlayer.IsPlaying)
            this.MainMediaPlayer.Play();
        this.ViewContent = false;
    }

    [RelayCommand]
    async Task AddToLearningContent()
    {
        var command = new AddStudyContentCommand()
        {
            PhraseId = this.CurrentPhrase?.Id ?? 0,
            Content = this.LearningContent,
            Translation = this.TranslateContent,
            StudyType = this.ContentType
        };

        var result = await this.mediator.Send(command);

        if (result.IsSuccess)
        {
            this.TranslateContent = "";
            this.ViewContent = false;
            this.LearningContent = "";
            this.MainMediaPlayer.Play();
            return;
        }
        this.ErrorMessages.Clear();
        foreach (var err in result.Errors)
        {
            this.ErrorMessages.Add(err.Message);
        }

    }
    ~MovieDetailsViewModel()
    {
        this.Dispose();
    }
}
