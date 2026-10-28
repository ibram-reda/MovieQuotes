namespace MovieQuotes.UI.ViewModels;

using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibVLCSharp.Shared;
using MovieQuotes.Application.Features.MoviePhrases.Models;
using MovieQuotes.Application.Features.MoviePhrases.Queries;
using MovieQuotes.Application.Features.MoviePhrases.QueriesHandlers;
using MovieQuotes.Application.Features.VideoClips.Queries;
using MovieQuotes.Application.Features.VideoClips.QueriesHandlers;
using MovieQuotes.Application.Features.StudyMaterials;
using MovieQuotes.UI.Features.Movies.MovieDetails;
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
    private readonly SearchForPhraseQueryHandler searchForPhraseHandler;
    private readonly VideoClipQueryHandler videoClipQueryHandler;
    private readonly AddNewStudyMaterialCommandHandler addNewStudyMaterialHandler;
    public MediaPlayer MainMediaPlayer => mediaService.VideoPlayer;

    [System.Obsolete("For design-time use only")]
    public PlaybackViewModel()
    {
    }

    public PlaybackViewModel(
        SearchForPhraseQueryHandler searchForPhraseHandler,
        VideoClipQueryHandler videoClipQueryHandler,
        AddNewStudyMaterialCommandHandler addNewStudyMaterialHandler,
        NavigationService nav,
        MediaService mediaService) : base(nav)
    {
        this.mediaService = mediaService;
        this.searchForPhraseHandler = searchForPhraseHandler;
        this.videoClipQueryHandler = videoClipQueryHandler;
        this.addNewStudyMaterialHandler = addNewStudyMaterialHandler;
    }
    [ObservableProperty] private string searchText = "";
    [ObservableProperty] private int searchCount = 0;
    public ObservableCollection<Phrase> ReadyToPlay { get; } = new();
    [ObservableProperty] Phrase? currentPlayingPhrase = null;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasPrevious))]
    [NotifyPropertyChangedFor(nameof(HasNext))]
    [NotifyCanExecuteChangedFor(nameof(PreviousCommand))]
    [NotifyCanExecuteChangedFor(nameof(NextCommand))]
    int currentPlayingIndex = 0;
    [ObservableProperty] bool isSearching = false;

    public override string Title => "Search for phrase";
    private uint PageNumber = 0;

    [RelayCommand(IncludeCancelCommand = true, AllowConcurrentExecutions = false)]
    private async Task search(CancellationToken token = default)
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            ErrorMessages.Add("Search text cannot be empty.");
            return;
        }
        if (IsSearching)
        {
            // old search is stile happen
            ErrorMessages.Add("Wait until the previous.");

            return;
        }
        ReadyToPlay.Clear();
        ErrorMessages.Clear();
        CurrentPlayingIndex = 0;
        CurrentPlayingPhrase = null;
        PageNumber = 0;
        await Load(this.SearchText, PageNumber, token: token);

    }

    [RelayCommand]
    private async Task GoMoviePage(int movieId)
    {
        await this.NavigationService.NavigateToAsync<MovieDetailViewModel>(movieId);
    }



    bool HasNext => this.CurrentPlayingIndex < this.ReadyToPlay.Count;
    bool HasPrevious { get => this.CurrentPlayingIndex > 1; }

    [RelayCommand(CanExecute = nameof(HasPrevious))]
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
        CreateStudyMaterialViewModel createStudyMaterial = new(
            CurrentPlayingPhrase?.Id ?? 0,
            CurrentPlayingPhrase?.Text ?? "",
            "",
            addNewStudyMaterialHandler);

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
    }

    [RelayCommand(CanExecute = nameof(HasNext))]
    void Next()
    {
        if (HasNext)
        {

            var phrase = ReadyToPlay[CurrentPlayingIndex++];
            PlayPhrase(phrase);
        }

        // load next data background
        if (CurrentPlayingIndex > ReadyToPlay.Count - 5)
        {
            if (ReadyToPlay.Count == SearchCount)
                return;
            if(IsSearching)
                return;
            Task.Run(async () => await Load(this.SearchText, ++PageNumber));
        }


    }

    private async Task  Load(string searchText, uint PageNumber = 0, uint pageSize = 20, CancellationToken token = default)
    {
        if (IsSearching)
        {
            return;
        }
        IsSearching = true;
        if (string.IsNullOrWhiteSpace(searchText))
        {
            ErrorMessages.Add("Search text cannot be empty.");
            IsSearching = false;
            return;
        }
        var query = new SearchForPhraseQuery(searchText)
        {
            ResultPerPage = pageSize,
            PageNumber = PageNumber
        };
        var result = await this.searchForPhraseHandler.Handle(query, token);

        if (!result.IsSuccess)
        {
            ErrorMessages.Add(result.Errors.First().Message ?? "Unknown error occurred");
            IsSearching = false;
            return ;
        }

        SearchCount = result.Count;

        foreach (var phrase in result?.Payload ?? [])
        {
            if (token.IsCancellationRequested)
                break;

            if (String.IsNullOrEmpty(phrase.VideoLocation))
            {
                var vc = new VideoClipQuery(phrase.Id);
                var r = await this.videoClipQueryHandler.Handle(vc, token);
                if (r.IsSuccess)
                {
                    phrase.VideoLocation = r.Payload ?? "";
                }                
            }
            ReadyToPlay.Add(phrase);
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                this.NextCommand.NotifyCanExecuteChanged();
            });
        }
        IsSearching = false;
        return ;
    }
}
