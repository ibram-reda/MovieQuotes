namespace MovieQuotes.UI.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibVLCSharp.Shared;
using MovieQuotes.Application.Features.MoviePhrases.Models;
using MovieQuotes.Application.Features.MoviePhrases.Queries;
using MovieQuotes.Application.Features.Movies.Queries;
using MovieQuotes.Application.Features.StudyPhrases.Commands;
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
        MainMediaPlayer = new(MainLibVLC)
        {
            EnableHardwareDecoding = true,
        };
        MainMediaPlayer.Opening += MainMediaPlayer_Opening;
        MainMediaPlayer.TimeChanged += MainMediaPlayer_TimeChanged;
        MainMediaPlayer.LengthChanged += MainMediaPlayer_LengthChanged;
        MainMediaPlayer.PositionChanged += MainMediaPlayer_PositionChanged;

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
        this.UpdateCurrentPhrase();
    }

    public override string Title => $"Details of {MovieName}";

    [ObservableProperty] int _id;
    [NotifyPropertyChangedFor(nameof(Title))]
    [ObservableProperty] string movieName = "";
    [ObservableProperty] string videoLocation = "";
    [ObservableProperty] long movieLength = 0;
    [ObservableProperty] private bool viewContent;
    List<Phrase> enPhrases = [];
    List<Phrase> arPhrases = [];
    [ObservableProperty] bool showArabicSubs = true;
    [ObservableProperty] Phrase? currentPhrase;
    [ObservableProperty] Phrase? currentArPhrase;
    [ObservableProperty] string learningContent = "";
    [ObservableProperty] string translateContent = "";
    [ObservableProperty] string contentType = "";
    public string[] AllowedType { get; } = ["noun", "adjective", "verb", "idiom", "phrasal verb", "phrase", "exclamation", "conjunction", "adverb"];

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
            this.enPhrases = result.Payload ?? [];
        }
        cmd.Language = Language.ar;
        result = await this.mediator.Send(cmd);
        if (result.IsSuccess)
        {
            this.arPhrases = result.Payload ?? [];
        }
    }

    public void Dispose()
    {
        this.MainMediaPlayer.Stop();
        // this.MainMediaPlayer.Dispose();
    }

    int enIndex = -1;
    int arIndex = -1;

    void UpdateCurrentPhrase()
    {
        var ctime = TimeSpan.FromMilliseconds(CurrentTime);
        if (enIndex + 1 < this.enPhrases.Count)
        {
            if (this.CurrentPhrase?.EndTime < ctime)
                CurrentPhrase = null;
            var a = this.enPhrases[enIndex + 1];
            if (a.StartTime <= ctime && ctime <= a.EndTime)
            {
                this.CurrentPhrase = a;
                enIndex++;
            }
        }

        if (ShowArabicSubs && arIndex + 1 < this.arPhrases.Count)
        {
            if (this.CurrentArPhrase?.EndTime < ctime)
                CurrentArPhrase = null;
            var a = this.arPhrases[arIndex + 1];
            if (a.StartTime <= ctime && ctime <= a.EndTime)
            {
                this.CurrentArPhrase = a;
                arIndex++;
            }
        }
    }

    private void ResyncCurrentPhrase()
    {
        var ctime = TimeSpan.FromMilliseconds(CurrentTime);
        var en = GetPhrase(this.enPhrases, ctime);
        if (en is not null)
        {
            this.CurrentPhrase = en.Value.value;
            this.enIndex = en.Value.index;
        }

        // early return if Arabic did not need to update.
        if (!ShowArabicSubs)
            return;

        var ar = GetPhrase(this.arPhrases, ctime);
        if (ar is not null)
        {
            this.CurrentArPhrase = ar.Value.value;
            this.arIndex = ar.Value.index;
        }
    }

    private static (Phrase value, int index)? GetPhrase(IEnumerable<Phrase> phraseList, TimeSpan currentTime)
    {
        var current = phraseList.Select((v, i) => new { v, i })
            .FirstOrDefault(a => a.v.StartTime <= currentTime && currentTime <= a.v.EndTime);

        if (current is not null)
        {
            return (current.v, current.i);
        }

        current = phraseList.Select((v, i) => new { v, i })
            .LastOrDefault(a => a.v.StartTime <= currentTime);
        if (current is null)
            return null;

        return (null!, current.i);
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
        var command = new CreateStudyPhraseCommand()
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
