namespace MovieQuotes.UI.ViewModels;

using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibVLCSharp.Shared;
using MediatR;
using MovieQuotes.Application.Features.MoviePhrases.Models;
using MovieQuotes.Application.Features.MoviePhrases.Queries;
using MovieQuotes.Application.Features.VideoClips.Queries;
using MovieQuotes.UI.Features.StudyMaterials;
using MovieQuotes.UI.Features.StudyMaterials.CreateStudyMaterial;
using MovieQuotes.UI.Services;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


public partial class PlaybackViewModel : PageViewModelBase
{
    private MediaService mediaService;
    public MediaPlayer MainMediaPlayer => mediaService.VideoPlayer;

    [System.Obsolete("For design-time use only")]
    public PlaybackViewModel()
    {    
    }

    public PlaybackViewModel(IMediator mediator, NavigationService nav, MediaService mediaService) : base(mediator, nav)
    {
        this.mediaService = mediaService;
    }
    [ObservableProperty] private string searchText = "";
    [ObservableProperty] private int searchCount = 0;
    public ObservableCollection<Phrase> Phrases { get; } = new();
    public ObservableCollection<Phrase> ReadyToPlay { get; } = new();
    [ObservableProperty] Phrase? currentPlayingPhrase = null;
    [ObservableProperty] int currentPlayingIndex = 0;
    private bool isSearching = false;

    public override string Title => "Search for phrase";
    private uint PageNumber =0;

    [RelayCommand(IncludeCancelCommand = true)]
    private async Task search(CancellationToken token = default)
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            ErrorMessages.Add("Search text cannot be empty.");
            return;
        }
        Phrases.Clear();
        ReadyToPlay.Clear();        
        ErrorMessages.Clear();
        CurrentPlayingIndex = 0;
        CurrentPlayingPhrase = null;
        PageNumber =0;
        await Load(this.SearchText,PageNumber);
        
        
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

    [RelayCommand]
    void CreateStudy()
    {
        CreateStudyMaterialViewModel createStudyMaterial = new(CurrentPlayingPhrase?.Id ?? 0, CurrentPlayingPhrase?.Text ?? "", "");
        
        NavigationService.NavigateToPopup(createStudyMaterial);
        createStudyMaterial.OnSaved += (IsSuccess, result) =>
        {
            if (!IsSuccess)
            {
                this.ErrorMessages.Clear();
                foreach (var err in result.Errors)
                {
                    this.ErrorMessages.Add(err.Message);
                }
            }
        };
    }

    void PlayPhrase(Phrase phrase)
    {
        this.CurrentPlayingPhrase = phrase;
        this.mediaService.PlayVideo(phrase.VideoLocation);

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

        // load next data background
        if(CurrentPlayingIndex > ReadyToPlay.Count - 5 && !isSearching)
        {
            Task.Run(async ()=>await Load(this.SearchText,++PageNumber));
        }


    }

    private async Task Load(string searchText,uint PageNumber=0,uint pageSize =20, CancellationToken token = default)
    {
        if(isSearching) return;
        isSearching = true;
        if (string.IsNullOrWhiteSpace(searchText))
        {
            ErrorMessages.Add("Search text cannot be empty.");
            return;
        }
        var query = new SearchForPhraseQuery(searchText)
        {
            ResultPerPage = pageSize,
            PageNumber = PageNumber
        };
        var result = await this.mediator.Send(query, token);

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
        isSearching = false;
    }
}
