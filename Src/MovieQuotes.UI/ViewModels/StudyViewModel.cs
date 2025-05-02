namespace MovieQuotes.UI.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibVLCSharp.Shared;
using MovieQuotes.Application.Features.StudyPhrases.Models;
using MovieQuotes.Application.Features.StudyPhrases.Queries;
using MovieQuotes.Application.Features.VideoClips.Queries;
using MovieQuotes.UI.Services;
using MovieQuotes.UI.ViewModels.Dialogues;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

    void PlayPhrase(StudyPhrase phrase)
    {
        this.CurrentPlayingPhrase = phrase;
        var uri = new Uri(phrase.VideoLocation);
        Media media = new Media(this.MainLibVLC, uri);
        MainMediaPlayer.Media?.Dispose();
        var r = MainMediaPlayer.Play(media);

        this.OnPropertyChanged(nameof(HasNext));
        this.OnPropertyChanged(nameof(HasPrevious));
    }

    [RelayCommand(CanExecute =nameof(IsDialogClosed))]
    public void EditPhrase()
    {
        this.ShowEditDialog = true;
        this.Dialogue = new PhraseEditDialogueViewModel();
        this.Dialogue.DialogueClosed += (_, _) => this.ShowEditDialog = false;
    }

    [RelayCommand]
    async Task Init()
    {
        var query = new GetAllStudyPhrasesGroupedQuery();
        var reslt = await this.mediator.Send(query);
        if (reslt.IsSuccess)
        {
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
        this.CurrentPlayingIndex = 0;

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

        this.PlayPhrase(this.Phrases[0]);
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
