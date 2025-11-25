namespace MovieQuotes.UI.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibVLCSharp.Shared;
using MediatR;
using MovieQuotes.Application.Features.MoviePhrases.Models;
using MovieQuotes.Application.Features.MoviePhrases.Queries;
using MovieQuotes.Application.Features.VideoClips.Queries;
using MovieQuotes.UI.Services;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


public partial class PlaybackViewModel : PageViewModelBase
{
    private LibVLC MainLibVLC { get; }
    public MediaPlayer MainMediaPlayer { get; }

    [System.Obsolete("For design-time use only")]
    public PlaybackViewModel()
    {    
    }

    public PlaybackViewModel(IMediator mediator, NavigationService nav) : base(mediator, nav)
    {
        MainLibVLC = new();
        MainMediaPlayer = new(MainLibVLC);
    }
    [ObservableProperty] private string searchText = "";
    [ObservableProperty] private int searchCount = 0;
    public ObservableCollection<Phrase> Phrases { get; } = new();
    public ObservableCollection<Phrase> ReadyToPlay { get; } = new();
    [ObservableProperty] Phrase? currentPlayingPhrase = null;
    [ObservableProperty] int currentPlayingIndex = 0;

    public override string Title => "Search for phrase";

    [RelayCommand(IncludeCancelCommand = true)]
    private async Task search(CancellationToken token = default)
    {
        ErrorMessages.Clear();
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            ErrorMessages.Add("Search text cannot be empty.");
            return;
        }
        Phrases.Clear();
        ReadyToPlay.Clear();
        CurrentPlayingIndex = 0;
        CurrentPlayingPhrase = null;
        var query = new SearchForPhraseQuery(SearchText)
        {
            ResultPerPage = 20,
        };
        IsBusy = true;
        var result = await this.mediator.Send(query, token);
        IsBusy = false;

        if (!result.IsSuccess)
        {
            ErrorMessages.Add(result.Errors.First().Message ?? "Unknown error occurred");
            return;
        }

        SearchCount = result.Count;
        foreach (var phrase in result?.Payload ?? [])
        {
            Phrases.Add(phrase);
        }
        
        foreach (var phrase in result.Payload ?? [])
        {
            if (token.IsCancellationRequested) 
                break; 
            var vc = new VideoClipQuery(phrase.Id);
            var r = await this.mediator.Send(vc);
            if (r.IsSuccess)
            {
                phrase.VideoLocation = r.Payload ?? "";
                Phrases.Remove(phrase);
                ReadyToPlay.Add(phrase);
            }

        }
    }

    bool HasNext => this.CurrentPlayingIndex+1 <= this.ReadyToPlay.Count;
    bool HasPrevious => this.CurrentPlayingIndex > 0;

    [RelayCommand]
    void Previous()
    {
        if (HasPrevious)
        {
            var phrase = this.ReadyToPlay[--CurrentPlayingIndex];
            PlayPhrase(phrase);
        }
    }

    void PlayPhrase(Phrase phrase)
    {
        this.CurrentPlayingPhrase = phrase;
        var uri = new Uri(phrase.VideoLocation);
        Media media = new Media(this.MainLibVLC, uri);
        MainMediaPlayer.Media?.Dispose();
        var r = MainMediaPlayer.Play(media);

        this.OnPropertyChanged(nameof(HasNext));
        this.OnPropertyChanged(nameof(HasPrevious));
    }

    [RelayCommand]
    void Next()
    {
        if (HasNext)
        {
            var phrase = ReadyToPlay[CurrentPlayingIndex++];
            PlayPhrase(phrase);
        }

    }
}
