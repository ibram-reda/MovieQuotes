namespace MovieQuotes.UI.Features.Movies.WatchMovie;

using Avalonia.Controls.Generators;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibVLCSharp.Shared;
using MediatR;
using MovieQuotes.Application.Features.MoviePhrases.Commands;
using MovieQuotes.Application.Features.MoviePhrases.Queries;
using MovieQuotes.Application.Features.Movies.Queries;
using MovieQuotes.UI.Features.StudyMaterials.CreateStudyMaterial;
using MovieQuotes.UI.Services;
using MovieQuotes.UI.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;


internal partial class WatchMovieViewModel : PageViewModelBase, IDisposable
{
    private LibVLC MainLibVLC { get; }
    public MediaPlayer MainMediaPlayer { get; }

    [System.Obsolete("For design-time use only")]
    public WatchMovieViewModel()
    {
    }

    public WatchMovieViewModel(IMediator mediator, NavigationService nav) : base(mediator, nav)
    {
        MainLibVLC = new();
        MainMediaPlayer = new(MainLibVLC)
        {
            EnableHardwareDecoding = false,
        };
        MainMediaPlayer.EnableHardwareDecoding = true;
        MainMediaPlayer.Opening += MainMediaPlayer_Opening;
        MainMediaPlayer.TimeChanged += MainMediaPlayer_TimeChanged;
        MainMediaPlayer.LengthChanged += MainMediaPlayer_LengthChanged;
        MainMediaPlayer.PositionChanged += MainMediaPlayer_PositionChanged;
        MainMediaPlayer.Forward += (_, _) => this.ResyncCurrentPhrase();
        MainMediaPlayer.Backward += (_, _) => this.ResyncCurrentPhrase();

    }


    private async void MainMediaPlayer_Opening(object? sender, EventArgs e)
    {
        await this.LoadSubtitlesAsync();
    }

    private void MainMediaPlayer_PositionChanged(object? sender, MediaPlayerPositionChangedEventArgs e)
    {
        var value = (long)(e.Position * MovieLength);
        this.SetProperty(ref this.currentTime, value, nameof(CurrentTime));
        this.ResyncCurrentPhrase();
    }

    private void MainMediaPlayer_LengthChanged(object? sender, MediaPlayerLengthChangedEventArgs e)
    {
        this.MovieLength = e.Length;
    }

    private void MainMediaPlayer_TimeChanged(object? sender, MediaPlayerTimeChangedEventArgs e)
    {
        this.SetProperty(ref this.currentTime, e.Time, nameof(CurrentTime));
        this.ResyncCurrentPhrase();
    }

    public override string Title => $"Details of {MovieName}";

    [ObservableProperty] int _id;
    [NotifyPropertyChangedFor(nameof(Title))]
    [ObservableProperty] string movieName = "";
    [ObservableProperty] string videoLocation = "";
    [ObservableProperty] long movieLength = 0;
    [ObservableProperty] private bool viewContent;
    [ObservableProperty] bool showArabicSubs = true;
    [ObservableProperty] bool canLoadEnSubs = false;
    [ObservableProperty] CreateStudyMaterialViewModel? createStudyMaterial;
    SubtitleManager? EnSubtitleManager = null;
    public SubtitleEntry? CurrentPhrase => this.EnSubtitleManager?.CurrentSubtitle;
    public SubtitleEntry? CurrentArPhrase => this.ArSubtitleManager?.CurrentSubtitle;
    SubtitleManager? ArSubtitleManager = null;

    private long currentTime = 0;

    public long CurrentTime
    {
        get => this.currentTime;
        set
        {
            MainMediaPlayer.Time = value;
            this.ResyncCurrentPhrase();
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

        var uri = new Uri(VideoLocation);
        Media media = new Media(this.MainLibVLC, uri);
        MainMediaPlayer.Media = media;
    }

    private async Task LoadSubtitlesAsync()
    {
        var cmd = new GetAllPhrasesForMovieQuery() { MovieId = this.Id };
        var result = await this.mediator.Send(cmd);
        if (result.IsSuccess)
        {
            this.EnSubtitleManager = new(result.Payload?.Select(a => new SubtitleEntry(a.Id, a.StartTime, a.EndTime, a.Text)) ?? []);
            this.EnSubtitleManager.OnSubtitleChanged += _ =>
                OnPropertyChanged(nameof(CurrentPhrase));
        }
        cmd.Language = Language.ar;
        result = await this.mediator.Send(cmd);
        if (result.IsSuccess)
        {
            this.ArSubtitleManager = new(result.Payload?.Select(a => new SubtitleEntry(a.Id, a.StartTime, a.EndTime, a.Text)) ?? []);
            this.ArSubtitleManager.OnSubtitleChanged += _ =>
                OnPropertyChanged(nameof(CurrentArPhrase));
        }

        ResyncCurrentPhrase();
    }

    public void Dispose()
    {
        this.MainMediaPlayer.Stop();
        // this.MainMediaPlayer.Dispose();
    }

    /// <summary>
    /// Resynchronizes the current phrase based on the current playback time.
    /// </summary>
    /// <remarks>This method updates the current phrase and its index for both English and Arabic subtitles, 
    /// depending on the playback time. If Arabic subtitles are disabled, only the English phrase is updated.</remarks>
    private void ResyncCurrentPhrase()
    {
        var ctime = TimeSpan.FromMilliseconds(MainMediaPlayer.Time);
        this.EnSubtitleManager?.Update(ctime);

        if (ShowArabicSubs)
            this.ArSubtitleManager?.Update(ctime);
    }

    [RelayCommand]
    void GoToPreviousPhrase()
    {
        if (this.EnSubtitleManager == null)
            return;
        var prev = this.EnSubtitleManager.GetPreviousPhrase(TimeSpan.FromMilliseconds(MainMediaPlayer.Time));
        if (prev != null)
        {
            CurrentTime = (long)prev.StartTime.TotalMilliseconds;
        }
    }

    [RelayCommand]
    void GoToNextPhrase()
    {
        if (this.EnSubtitleManager == null)
            return;
        var next = this.EnSubtitleManager.GetNextPhrase(TimeSpan.FromMilliseconds(MainMediaPlayer.Time));
        if (next != null)
        {
            CurrentTime = (long)next.StartTime.TotalMilliseconds;
        }
    }

    [RelayCommand]
    async Task GoToPrevious()
    {
        this.MainMediaPlayer.Stop();
        this.NavigationService.GoBack();
    }

    [RelayCommand]
    async Task OpenPopUp()
    {
        
        if (this.MainMediaPlayer.IsPlaying)
            this.MainMediaPlayer.Pause();
        this.CreateStudyMaterial = new(CurrentPhrase?.Id ?? 0, CurrentPhrase?.Text ?? "", CurrentArPhrase?.Text ?? "");
        this.CreateStudyMaterial.OnSaved += (IsSuccess, result) =>
        {
            if (!IsSuccess)
            {
                this.ErrorMessages.Clear();
                foreach (var err in result.Errors)
                {
                    this.ErrorMessages.Add(err.Message);
                }
            }
            ClosePopUp();
        };
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


    [RelayCommand(AllowConcurrentExecutions = false)]
    async Task LoadEnPhrases()
    {
        if (this.CanLoadEnSubs)
        {
            var cmd = new InsertPhrasesForMovieCommand() { MovieId = this.Id };
            var result = await this.mediator.Send(cmd);

            if (result.IsSuccess)
            {
                await LoadSubtitlesAsync();
            }

            if (result.IsError)
            {
                this.ErrorMessages.Clear();
                foreach (var err in result.Errors)
                {
                    this.ErrorMessages.Add(err.Message);
                }
            }

            CanLoadEnSubs = false;

        }
    }
    ~WatchMovieViewModel()
    {
        this.Dispose();
    }
}
