namespace MovieQuotes.UI.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibVLCSharp.Shared;
using MovieQuotes.Application.Features.MoviePhrases.Commands;
using MovieQuotes.Application.Features.StudyPhrases.Models;
using MovieQuotes.Application.Features.StudyPhrases.Queries;
using MovieQuotes.Application.Features.VideoClips.Queries;
using MovieQuotes.UI.Extensions;
using MovieQuotes.UI.Services;
using MovieQuotes.UI.ViewModels.Dialogues;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;


public partial class StudyViewModel : ViewModelBase
{

    [ObservableProperty] StudyPhrase? currentPlayingPhrase;
    [NotifyCanExecuteChangedFor(nameof(EditPhraseCommand))]
    [ObservableProperty] bool showEditDialog = false;
    public bool IsDialogClosed => !ShowEditDialog;
    [ObservableProperty] StudyPhrasesGroupByMovie? selectedMovie;
    [ObservableProperty] DialogueViewModelBase? dialogue;

    [ObservableProperty] int currentPlayingIndex;
    public ObservableCollection<StudyPhrase> Phrases { get; } = new();
    public ObservableCollection<StudyPhrasesGroupByMovie> Movies { get; } = [];

    private LibVLC MainLibVLC { get; }
    public MediaPlayer MainMediaPlayer { get; }
    private IFilesService _filesService;

    public override string Title => "Study";

    public StudyViewModel()
    {
        MainLibVLC = new();
        MainMediaPlayer = new(MainLibVLC)
        {
            EnableHardwareDecoding = true,
        };
        this._filesService = this.GetService<IFilesService>();
    }


    [RelayCommand]
    void Next()
    {
        if (!HasNext) return;
        var phrase = Phrases[++CurrentPlayingIndex];
        PlayPhrase(phrase);
    }

    [RelayCommand]
    void ShowFolder() =>
        this._filesService.ExploreFile(CurrentPlayingPhrase?.VideoLocation ?? "");


    bool HasNext => this.CurrentPlayingIndex < this.Phrases.Count - 1;
    bool HasPrevious => this.CurrentPlayingIndex > 0;

    [RelayCommand]
    void Previous()
    {
        if (!HasPrevious) return;
        var phrase = this.Phrases[--CurrentPlayingIndex];
        PlayPhrase(phrase);
    }

    void PlayPhrase(int index)
    {
        this.CurrentPlayingPhrase = null;
        if (index < 0 || index >= this.Phrases.Count) return;
        var phrase = this.Phrases[index];
        PlayPhrase(phrase);
    }


    [RelayCommand]
    void Shuffle()
    {
        this.Phrases.Shuffle();
        PlayPhrase(0);
    }
    void PlayPhrase(StudyPhrase phrase)
    {
        this.CurrentPlayingIndex = this.Phrases.IndexOf(phrase);
        this.CurrentPlayingPhrase = phrase;
        var uri = new Uri(phrase.VideoLocation);
        Media media = new Media(this.MainLibVLC, uri);        
        MainMediaPlayer.Media?.Dispose(); 
        var r = MainMediaPlayer.Play(media);
   
        this.OnPropertyChanged(nameof(HasNext));
        this.OnPropertyChanged(nameof(HasPrevious));
    }

    [RelayCommand(CanExecute = nameof(IsDialogClosed))]
    public void EditPhrase()
    {
        this.Dialogue = new PhraseEditDialogueViewModel(CurrentPlayingPhrase);
        this.Dialogue.DialogueClosed += async (sender, args) =>
        {
            this.IsBusy = true;
            this.ShowEditDialog = false;
            if (args.IsClosedSuccessfully && sender is PhraseEditDialogueViewModel newPhrase)
            {
                // Update the phrase in the data source
                var updateCommand = new EditPhraseCommand()
                {
                    PhraseId = newPhrase.PhraseId,
                    PhraseText = newPhrase.PhraseText,
                    StartTime = newPhrase.StartTime,
                    EndTime = newPhrase.EndTime
                };

                var result = await this.mediator.Send(updateCommand);

                if (result.IsError)
                    this.ErrorMessages.Add("Failed to update phrase: " + result.Errors.First().Message);

                if (result.IsSuccess)
                {
                    // reload the phrases to refresh the list.
                    var index = this.CurrentPlayingIndex;
                    await this.GetPhrases();
                    PlayPhrase(index); // Re-play the phrase after update
                }

            }
            this.IsBusy = false;
        };
        this.ShowEditDialog = true;
    }



    [RelayCommand]
    async Task Init()
    {
        var query = new GetAllStudyPhrasesGroupedQuery();
        var reslt = await this.mediator.Send(query);
        if (reslt.IsSuccess)
        {
            this.Movies.Clear();
            this.Movies.Add(new() {
                MovieName = "random phrases",
                StudyCount = reslt.Payload?.Sum(x => x.StudyCount) ?? 0,
            });
            foreach (var m in reslt.Payload ?? [])
                Movies.Add(m);
        }

    }

    [RelayCommand]
    void Replay()
    {
        this.MainMediaPlayer.Stop();
        this.MainMediaPlayer.Play();
    }

    [RelayCommand]
    async Task GetPhrases()
    {
        this.Phrases.Clear();
        this.CurrentPlayingPhrase = null;

        var qry = new GetAllStudyPhrasesQuery();
        if (SelectedMovie is not null)
            qry.MovieId = SelectedMovie.MovieId;
        var result = await this.mediator.Send(qry);

        if (result.IsError)
        {
            this.ErrorMessages.Add("can't load phrases");
            return;
        }

        var lst = result.Payload;
        foreach (var phrase in lst)
        {
            if (string.IsNullOrWhiteSpace(phrase.VideoLocation))
                await GenerateVideoAsync(phrase);
            this.Phrases.Add(phrase);
        }
        PlayPhrase(0); // Play the first phrase by default if available
    }

    async Task GenerateVideoAsync(StudyPhrase phrase)
    {
        var q = new VideoClipQuery(phrase.PhraseId);
        var videoClipResult = await this.mediator.Send(q);
        if (videoClipResult.IsSuccess)
        {
            phrase.VideoLocation = videoClipResult.Payload!;
        }
        else
        {
            this.ErrorMessages.Add(videoClipResult.Errors.First().Message);
        }
    }
}
